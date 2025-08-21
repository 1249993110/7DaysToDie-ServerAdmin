using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.WebApi.Authentication;
using LSTY.Sdtd.ServerAdmin.WebApi.Controllers;
using LSTY.Sdtd.ServerAdmin.WebApi.DataProtection;
using LSTY.Sdtd.ServerAdmin.WebApi.Middlewares;
using LSTY.Sdtd.ServerAdmin.WebApi.Providers;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag;
using NSwag.AspNet.Owin;
using NSwag.Generation.Processors.Security;
using Owin;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace LSTY.Sdtd.ServerAdmin.WebApi
{
    /// <summary>
    /// The Startup class is specified as a type parameter in the WebApp.Start method.
    /// </summary>
    internal class Startup
    {
        /// <summary>
        /// OAuth token endpoint path
        /// </summary>
        public const string OAuthTokenEndpointPath = "/api/oauth/token";

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            Converters = new List<JsonConverter>()
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            },
        };

        /// <summary>
        /// This code configures Web API.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host.
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new BsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = _jsonSerializerSettings;
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); // Default return json
            config.Filters.Add(new ValidateModelAttribute());

            config.Services.Replace(typeof(IHttpControllerTypeResolver), new CustomHttpControllerTypeResolver());
            config.Services.Replace(typeof(IExceptionHandler), new PassThroughExceptionHandler());

            app.Use<GlobalExceptionHandleMiddleware>(_jsonSerializerSettings);

            app.Use<SteamReturnMiddleware>();

            string webRootPath = Path.Combine(ModMain.ModInstance.Path, "wwwroot");
            if (Directory.Exists(webRootPath))
            {
                var fileSystem = new PhysicalFileSystem(webRootPath);
                // Serve the default file, if present.
                app.UseDefaultFiles(new DefaultFilesOptions()
                {
                    DefaultFileNames = new string[] { "index.html" },
                    FileSystem = fileSystem,
                    RequestPath = PathString.Empty
                });
                // Serve static files.
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileSystem = fileSystem,
                    RequestPath = PathString.Empty
                });
            }

            app.UseSwaggerUi(Assembly.GetExecutingAssembly(), settings =>
            {
                // configure settings here
                // settings.GeneratorSettings.*: Generator settings and extension points
                // settings.*: Routing and UI settings

                // You can set it to load from the annotation file, but the loaded content can be overwritten by the OpenApiTagAttribute attribute
                settings.GeneratorSettings.UseControllerSummaryAsTagDescription = true;
                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("Basic Auth",
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.Basic,
                        In = OpenApiSecurityApiKeyLocation.Header,
                    }));
                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token",
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = OpenApiSecurityApiKeyLocation.Header,
                    }));
                settings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("Basic Auth", "JWT Token"));
                settings.GeneratorSettings.ApplySettings(new NewtonsoftJsonSchemaGeneratorSettings()
                { 
                    SerializerSettings = _jsonSerializerSettings, SchemaType = SchemaType.OpenApi3 
                }, null);
                settings.PostProcess = (document) =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "7DaysToDie-ServerAdmin Web APIs Documentation";
                    document.Info.Description = "Rest APIs Documentation for 7 Days to Die dedicated servers.";
                    document.Info.TermsOfService = "https://7dtd.top";
                    document.Info.Contact = new OpenApiContact()
                    {
                        Name = "LuoShuiTianTi",
                        Email = "1249993110@qq.com",
                        Url = "https://github.com/1249993110"
                    };
                    document.Info.License = new OpenApiLicense()
                    {
                        Name = "LICENSE",
                        Url = "https://github.com/1249993110/7DaysToDie-ServerAdmin/blob/main/README.md"
                    };

                    AddOAuthTokenEndpointApiSchema(document);
                };
            });

            app.SetDataProtectionProvider(new CustomDataProtectionProvider());
            // Token Generation
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString(OAuthTokenEndpointPath),
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(AppConfig.Settings.AccessTokenExpireTime),
                Provider = new CustomOAuthProvider(),
                RefreshTokenProvider = new CustomRefreshTokenProvider()
            });

            app.Use<BasicAuthenticationMiddleware>(new BasicAuthenticationOptions()
            {
                Realm = Common.ProductName,
                Verifier = (username, password) =>
                {
                    // Verify the username and password
                    return AppConfig.Settings.UserName == username && AppConfig.Settings.Password == password;
                }
            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            app.Use<ServerSentEventsMiddleware>(_jsonSerializerSettings);

            //app.Use(async (context, next) =>
            //{
            //    if (ModMain.IsGameStartDone == false)
            //    {
            //        var error = new { Message = "The game is still initializing." };
            //        context.Response.ContentType = "application/json";
            //        context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            //        string json = JsonConvert.SerializeObject(error, _jsonSerializerSettings);
            //        await context.Response.WriteAsync(json);
            //    }
            //    else
            //    {
            //        await next();
            //    }
            //});

            // Adds the Web API runtime (middleware) to the OWIN pipeline, responsible for handling HTTP requests and routing them to the correct Web API controllers and action methods
            app.UseWebApi(config);
            config.EnsureInitialized();
        }

        private static void AddOAuthTokenEndpointApiSchema(OpenApiDocument document)
        {
            var tokenOpr = new OpenApiOperation()
            {
                OperationId = "OAuth_Token",
                Consumes = new List<string>() { "application/x-www-form-urlencoded" },
                Produces = new List<string>() { "application/json" },
                Tags = new List<string>() { "Authentication" },
                Summary = "User login with form data",
                Description = "Get the access token used for webapp.",
                RequestBody = new OpenApiRequestBody()
                {
                    Description = "User login with form data",
                    IsRequired = true,
                }
            };

            tokenOpr.RequestBody.Content["application/x-www-form-urlencoded"] = new OpenApiMediaType()
            {
                Schema = new JsonSchema()
                {
                    Type = JsonObjectType.Object,
                    Properties =
                    {
                        ["grant_type"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true, Default = "password" },
                        ["username"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = false },
                        ["password"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = false },
                        ["refresh_token"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = false }
                    }
                }
            };

            var resp200 = new OpenApiResponse()
            {
                Description = "Authentication token and meta data.",
                Schema = new JsonSchema()
            };
            resp200.Schema.Properties.Add("access_token", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });
            resp200.Schema.Properties.Add("token_type", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });
            resp200.Schema.Properties.Add("expires_in", new JsonSchemaProperty { Type = JsonObjectType.Integer, IsRequired = true, Format = JsonFormatStrings.Integer });
            resp200.Schema.Properties.Add("refresh_token", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = false });

            var resp400 = new OpenApiResponse()
            {
                Description = "Authentication error.",
                Schema = new JsonSchema()
            };
            resp400.Schema.Properties.Add("error", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });
            resp400.Schema.Properties.Add("error_description", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });

            tokenOpr.Responses.Add("200", resp200);
            tokenOpr.Responses.Add("400", resp400);

            var path = new OpenApiPathItem();
            path.Add(OpenApiOperationMethod.Post, tokenOpr);
            document.Paths.Add(OAuthTokenEndpointPath, path);
        }
    }
}
