using Cib.Markets.Core.Correlation.AspNetCore;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Hangfire;
using Serilog;
using CleanArchitecture.Api.Filters;

namespace CleanArchitecture.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        readonly IConfiguration _configuration;
        readonly IWebHostEnvironment _environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Log.Logger);
            services.AddHttpContextAccessor();
            services.AddCustomCors().AddCustomMvc().AddCustomApiFeatures().AddCustomConfiguration()
                .AddCustomHealthChecks()
                .AddCorrelationId()
                .AddFeatureManagement();

            services.AddIntegrations(_configuration);
            services.AddApplication(_configuration);
            services.AddInfrastructure(_configuration);
     
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app
                .UseHttpsRedirection()
                .UseCorrelationId()
                .UseSerilogRequestLogging()
                .UseStaticFiles()
                .ConfigureCors()
                .ConfigureSwagger(provider)
                .UseRouting()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapHangfireDashboard();

                    endpoints.ConfigureHealthChecks();
                });
        }
    }
}
