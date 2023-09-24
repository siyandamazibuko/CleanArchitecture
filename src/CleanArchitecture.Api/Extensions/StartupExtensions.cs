using System;
using System.Linq;
using System.Text.Json.Serialization;
using Cib.Markets.Core.AspNetCore.Filters;
using Cib.Markets.Core.AspNetCore.Swagger;
using Cib.Markets.Core.AspNetCore.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using CleanArchitecture.Api.Filters;
using CleanArchitecture.Api.Middleware;
using CleanArchitecture.Infrastructure.Constants;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using CleanArchitecture.Api.HealthChecks;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;

namespace CleanArchitecture.Api.Extensions
{
    public static class StartupExtensions
    {
        #region ConfigureService

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    policy => policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );
            });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.EnableEndpointRouting = true;
                    options.Filters.Add(new AuthorizeFilter());
                    options.Filters.Add<ActionLogFilterAttribute>();
                    options.Filters.Add(typeof(ValidatorActionFilter));
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddControllersAsServices();

            return services;
        }

        public static IServiceCollection AddCustomApiFeatures(this IServiceCollection services)
            => services
                .AddValidationErrorLogging()
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                })
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddRouting(options =>
                {
                    options.LowercaseUrls = true;
                });

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            services
                .AddCustomSwaggerGen(options =>
                {
                    configuration
                        .Bind(ApiConfiguration.Swagger, options);
                });

            // services
            //     .AddCustomSwaggerGen(environment, options =>
            //     {
            //         configuration
            //             .Bind(ApiConfiguration.Swagger, options);
            //     });

            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
        {
            services
                .AddOptions()
                .Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Instance = context.HttpContext.Request.Path,
                        };

                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json", "application/problem+xml" }
                        };
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks().AddCheck<SQSConsumerHealthCheck>("SQSConsumerHealthCheck", HealthStatus.Unhealthy);

            // NOTE: More health checks can be added here

            return services;
        }

        #endregion

        #region ConfigureApp

        public static void ConfigureHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
             
            });
        }

        public static IApplicationBuilder ConfigureCors(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");

            return app;
        }

        public static IApplicationBuilder ConfigureRequestLogging(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<RequestLoggingMiddleware>();
        }

        public static IApplicationBuilder ConfigureSwagger(
            this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger(options => options.RouteTemplate = "/{documentName}/swagger.json");


            var options = new HealthCheckOptions();
            options.ResultStatusCodes[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable;

            options.ResponseWriter = async (ctx, rpt) =>
            {
                var result = JsonConvert.SerializeObject(new
                {
                    status = rpt.Status.ToString(),
                    errors = rpt.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                }, Formatting.None, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                ctx.Response.ContentType = MediaTypeNames.Application.Json;
                await ctx.Response.WriteAsync(result);
            };
            app.UseHealthChecks("/hc", options);

            app.UseSwaggerUI(options =>
            {
                foreach (ApiVersionDescription versionDescription in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint("/" + versionDescription.GroupName + "/swagger.json", versionDescription.GroupName.ToUpperInvariant());
                    options.RoutePrefix = string.Empty;
                }
            });
            return app;
        }

        #endregion
    }
}
