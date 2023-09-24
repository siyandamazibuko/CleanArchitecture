using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CleanArchitecture.UnitTests.Common
{
    [TestClass]
    public class ValidationTestBase
    {
        private static IConfigurationRoot _configuration;
        private static IServiceScopeFactory _scopeFactory;

        [TestInitialize]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"))
                .AddJsonFile("test.settings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
            
            var services = new ServiceCollection();
            
            services.AddLogging();

            
            services.AddApplication(_configuration);
            
            services.AddInfrastructure(_configuration);

            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        protected static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetService<ISender>();

            return await mediator.Send(request);
        }
    }
}
