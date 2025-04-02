using FastExpressionCompiler;
using LSTY.Sdtd.ServerAdmin.WebApi.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;
using NSwag;
using Serilog;
using Serilog.Events;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateBootstrapLogger();

            Log.Information("Starting up!");

            try
            {
                TypeAdapterConfig.GlobalSettings.Compiler = exp => exp.CompileFast();

                var builder = WebApplication.CreateBuilder(args);
                var env = builder.Environment;
                var config = builder.Configuration;
                var services = builder.Services;

                #region Services

                // Add services to the container.
                services.AddControllers();

                #region Serilog

                services.AddSerilog((services, lc) => lc
                        .ReadFrom.Configuration(config)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext());

                #endregion

                #region Swagger

                bool enableSwagger = config.GetValue<bool>("EnableSwagger");
                if (enableSwagger)
                {
                    // Register the Swagger services
                    services.AddOpenApiDocument(config =>
                    {
                        config.PostProcess = document =>
                        {
                            document.Info.Version = "v1";
                            document.Info.Title = "ASP.NET Core WebApi Documentation";
                            document.Info.Description = "";
                            document.Info.TermsOfService = "Discord";
                            document.Info.Contact = new OpenApiContact()
                            {
                                Name = "LuoShuiTianYi",
                                Email = "IceCoffee1024@outlook.com",
                                Url = "https://github.com/1249993110/7DaysToDie-ServerAdmin"
                            };
                            document.Info.License = new OpenApiLicense()
                            {
                                Name = "LICENSE",
                                Url = "https://github.com/1249993110/7DaysToDie-ServerAdmin/blob/main/LICENSE.md"
                            };
                        };

                        // You can set it to load from an annotation file, but the loaded content can be overwritten by the OpenApiTagAttribute attribute.
                        config.UseControllerSummaryAsTagDescription = true;

                        //config.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                        //{
                        //    Type = OpenApiSecuritySchemeType.Http,
                        //    Scheme = JwtBearerDefaults.AuthenticationScheme,
                        //    BearerFormat = "JWT",
                        //    Description = "Type into the textbox: {your JWT token}."
                        //});
                    });
                }

                #endregion

                #region ForwardedHeader

                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });

                #endregion

                #region Cors

                bool enableCors = config.GetValue<bool>("EnableCors");
                if (enableCors)
                {
                    var allowedOrigins = config.GetValue<string[]>("AllowedOrigins");

                    services.AddCors(options =>
                    {
                        options.AddPolicy("Cors", builder =>
                        {
                            if (allowedOrigins == null || allowedOrigins.Length == 0)
                            {
                                builder.AllowAnyOrigin();
                            }
                            else
                            {
                                builder.WithOrigins(allowedOrigins);
                            }
                            builder.AllowAnyHeader();
                            builder.AllowAnyMethod();
                        });
                    });
                }

                #endregion

                #endregion

                var app = builder.Build();

                #region Pipeline
                // Configure the HTTP request pipeline.

                bool enableRequestLog = config.GetValue<bool>("EnableRequestLog");
                if (enableRequestLog)
                {
                    app.UseSerilogRequestLogging();
                }

                app.UseMiddleware<GlobalExceptionHandleMiddleware>();
                app.UseForwardedHeaders();

                string? pathBase = config.GetValue<string>("PathBase");
                if (string.IsNullOrEmpty(pathBase) == false)
                {
                    app.UsePathBase(pathBase);
                }

                if (enableSwagger)
                {
                    // Register the Swagger endpoint and the Swagger UI middlewares
                    app.UseOpenApi(config =>
                    {
                        config.PostProcess = (document, httpRequest) =>
                        {
                            document.BasePath = httpRequest.PathBase;
                        };
                    });
                    app.UseSwaggerUi();
                }

                if(Directory.Exists(env.WebRootPath))
                {
                    var options = new DefaultFilesOptions();
                    options.DefaultFileNames.Clear();
                    options.DefaultFileNames.Add("index.html");
                    app.UseDefaultFiles(options);

                    app.UseStaticFiles(new StaticFileOptions()
                    {
                        OnPrepareResponse = (context) =>
                        {
                            if (context.File.Name == "index.html")
                            {
                                context.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate, proxy-revalidate, max-age=0";
                            }
                        }
                    });
                }

                app.UseRouting();

                if (enableCors)
                {
                    app.UseCors("Cors");
                }

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                #endregion

                app.Run();

                Log.Information("Stopped cleanly");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
