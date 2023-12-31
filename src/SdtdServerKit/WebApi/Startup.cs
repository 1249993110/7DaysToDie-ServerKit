﻿using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using NSwag.AspNet.Owin;
using NSwag.Generation.Processors.Security;
using NSwag;
using SdtdServerKit.WebApi.Providers;
using NJsonSchema;
using Microsoft.Owin.Security.DataProtection;
using SdtdServerKit.DataProtection;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using HarmonyLib;
using NJsonSchema.Generation;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;

namespace SdtdServerKit.WebApi
{
    /// <summary>
    /// The Startup class is specified as a type parameter in the WebApp.Start method.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// OAuth token endpoint path
        /// </summary>
        public const string OAuthTokenEndpointPath = "/api/oauth/token";

        /// <summary>
        /// This code configures Web API.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host.
            var config = new HttpConfiguration()
            {
                IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
            };
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new BsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = ModApi.JsonSerializerSettings;
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); // Default return json
            config.Filters.Add(new ValidateModelAttribute());

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    CustomLogger.Error(ex, "Error in Owin Host GlobalExceptionHandleMiddleware.");
                }
            });

            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new[] { "index.html" },
                RequestPath = PathString.Empty
            });

            string webRootPath = Path.Combine(ModApi.ModInstance.Path, "wwwroot");
            if (Directory.Exists(webRootPath))
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileSystem = new PhysicalFileSystem(webRootPath),
                    RequestPath = PathString.Empty
                });
            }
           
            app.UseSwaggerUi3(typeof(Startup).Assembly, settings =>
            {
                // configure settings here
                // settings.GeneratorSettings.*: Generator settings and extension points
                // settings.*: Routing and UI settings
                
                settings.GeneratorSettings.OperationProcessors.Add(new SdtdServerKit.WebApi.OperationSecurityScopeProcessor("JWT Token"));
                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token",
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = OpenApiSecurityApiKeyLocation.Header,
                    }));
                settings.GeneratorSettings.SerializerSettings = ModApi.JsonSerializerSettings;
                settings.PostProcess = (document) =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "7DaysToDie-ServerKit RESTful APIs Documentation";
                    document.Info.Description = "RESTful APIs Documentation for 7 Days to Die dedicated servers.";
                    document.Info.TermsOfService = "https://7daystodie.top";
                    document.Info.Contact = new OpenApiContact()
                    {
                        Name = "LuoShuiTianTi",
                        Email = "1249993110@qq.com",
                        Url = "https://github.com/1249993110"
                    };
                    document.Info.License = new OpenApiLicense()
                    {
                        Name = "LICENSE",
                        Url = "https://github.com/1249993110/7DaysToDie-ServerKit/blob/main/README.md"
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
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(ModApi.AppSettings.AccessTokenExpireTime),
                Provider = new SimpleAuthorizationServerProvider(),
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            app.Use(async (context, next) =>
            {
                if (ModApi.IsGameStartDone == false)
                {
                    string json = JsonConvert.SerializeObject(
                        new InternalServerError() { Message = "The game is still initializing." }, ModApi.JsonSerializerSettings);
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    await next();
                }
            });
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
                Tags = new List<string>() { "OAuth" },
                Description = "Get the access token used for webapp."
            };

            tokenOpr.Parameters.Add(new OpenApiParameter()
            {
                Name = "grant_type",
                Description = "",
                IsRequired = true,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.String,
                Default = "password"
            });
            tokenOpr.Parameters.Add(new OpenApiParameter()
            {
                Name = "username",
                Description = "",
                IsRequired = true,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.String
            });
            tokenOpr.Parameters.Add(new OpenApiParameter()
            {
                Name = "password",
                Description = "",
                IsRequired = true,
                Kind = OpenApiParameterKind.FormData,
                Type = JsonObjectType.String
            });

            var resp200 = new OpenApiResponse()
            {
                Description = "Authentication token and meta data.",
                Schema = new JsonSchema()
            };
            resp200.Schema.Properties.Add("access_token", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });
            resp200.Schema.Properties.Add("token_type", new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true });
            resp200.Schema.Properties.Add("expires_in", new JsonSchemaProperty { Type = JsonObjectType.Integer, IsRequired = true, Format = JsonFormatStrings.Integer });

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
