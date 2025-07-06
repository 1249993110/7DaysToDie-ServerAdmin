using Dapper;
using FastExpressionCompiler;
using IceCoffee.Db4Net.DependencyInjection;
using IceCoffee.Db4Net.SqliteTypeHandlers;
using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Enums;
using LSTY.Sdtd.ServerAdmin.Data.Logging;
using LSTY.Sdtd.ServerAdmin.RpcClient.Extensions;
using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.WebApi.Authentication;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.JsonConverters;
using LSTY.Sdtd.ServerAdmin.WebApi.Middlewares;
using LSTY.Sdtd.ServerAdmin.WebApi.NotificationPublishers;
using LSTY.Sdtd.ServerAdmin.WebApi.OperationProcessors;
using LSTY.Sdtd.ServerAdmin.WebApi.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Data.Sqlite;
using NSwag;
using Serilog;
using Serilog.Events;
using System.Data;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;

[assembly: ApiController]
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
                //TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);

                var builder = WebApplication.CreateBuilder(args);
                var env = builder.Environment;
                var config = builder.Configuration;
                var services = builder.Services;

                #region Services
                services.AddDbConnection<Repository>(string.Empty, "DbConnectionOptions:Default");

                // Add services to the container.
                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
                        options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                        options.JsonSerializerOptions.Converters.Add(new DateTimeNullableConverter());
                    });

                services.AddRpcClientManager<RpcClientConfigProvider>();

                services.AddSingleton<FunctionManager>();
                services.AddSingleton<IFunctionSettingsProvider, FunctionSettingsProvider>();
                services.AddSingleton<ICustomLoggerFactory, CustomLoggerFactory>();

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(typeof(FunctionManager).Assembly);
                    cfg.NotificationPublisherType = typeof(ParallelForeachPublisher);
                    cfg.LicenseKey = config.GetValue<string>("MediatR:LicenseKey");
                });

                services.AddMemoryCache();

                var basicAuthenticationSchemeOptions = config.GetRequiredSection(nameof(BasicAuthenticationSchemeOptions)).Get<BasicAuthenticationSchemeOptions>();
                ArgumentNullException.ThrowIfNull(basicAuthenticationSchemeOptions);
                
                services.AddAuthentication(AuthenticationSchemes.BasicAuthenticationScheme)
                   .AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationSchemes.BasicAuthenticationScheme, null, options =>
                   {
                       options.Realm = basicAuthenticationSchemeOptions.Realm ?? Shared.Constants.Common.CompanyName;
                       options.UserName = basicAuthenticationSchemeOptions.UserName;
                       options.Password = basicAuthenticationSchemeOptions.Password;
                   });

                services.AddSingleton<IAuthorizationHandler, GameServerOwnerHandler>();
                services.AddAuthorizationBuilder()
                    .AddPolicy(AuthorizationPolicys.GameServerOwner, policy =>
                    {
                        policy.Requirements.Add(new GameServerOwnerRequirement());
                    })
                    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build());

               // services.AddResponseCompression(options =>
               // {
               //     options.Providers.Add<BrotliCompressionProvider>();
               //     options.Providers.Add<GzipCompressionProvider>();
               //     options.EnableForHttps = true; // Enable for HTTPS responses
               //     options.MimeTypes = new[] { "application/json" };
               // });

               // services.Configure<BrotliCompressionProviderOptions>(options =>
               // {
               //     options.Level = CompressionLevel.Optimal;
               // });

               //services.Configure<GzipCompressionProviderOptions>(options =>
               // {
               //     options.Level = CompressionLevel.Optimal;
               // });

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

                        config.OperationProcessors.Add(new AddGameServerIdHeaderParameter());
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

                InitDatabase(app.Services.GetRequiredService<Repository>(), app.Environment);

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

                if (Directory.Exists(env.WebRootPath))
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

                app.UseWebSockets(new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromMinutes(2)
                });

                if (enableCors)
                {
                    app.UseCors("Cors");
                }

                app.UseAuthentication();
                app.UseAuthorization();

                //app.UseResponseCompression();
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
       
        private static void InitDatabase(Repository repository, IWebHostEnvironment env)
        {
            Log.Information("Initializing database...");

            SqlMapper.AddTypeHandler(new GuidHandler());
            try
            {
                using (var dbConnection = repository.CreateDbConnection())
                {
                    var fileInfo = new FileInfo(dbConnection.DataSource);
                    if (fileInfo.Directory?.Exists == false)
                    {
                        fileInfo.Directory.Create();
                    }

                    string path;
                    if (env.IsDevelopment())
                    {
                        path = Path.Combine(env.ContentRootPath, "../LSTY.Sdtd.ServerAdmin.Data/sql");
                    }
                    else
                    {
                        path = Path.Combine(AppContext.BaseDirectory, "sql");
                    }

                    if (Directory.Exists(path) == false)
                    {
                        Log.Error("SQL directory does not exist: {Path}", path);
                        return;
                    }

                    dbConnection.Open();
                    using (var dbTransaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var sqlPath in Directory.GetFiles(path, "*.sql").Order())
                            {
                                string sql = File.ReadAllText(sqlPath, Encoding.UTF8);
                                dbConnection.Execute(sql, transaction: dbTransaction);
                            }

                            dbTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbTransaction.Rollback();
                            throw;
                        }
                    }
                }

                Log.Information("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while initializing the database.");
            }
        }
    }
}
