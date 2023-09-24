using CleanArchitecture.Infrastructure;
using CleanArchitecture.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Serilog;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
                .AddFeatureManagement();

            services.AddIntegrations(_configuration);
            services.AddApplication(_configuration);
            services.AddInfrastructure(_configuration);
     
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .ConfigureCors()
                .ConfigureSwagger(provider)
                .UseRouting()
                .UseAuthentication()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.ConfigureHealthChecks();
                });
        }
    }
}
