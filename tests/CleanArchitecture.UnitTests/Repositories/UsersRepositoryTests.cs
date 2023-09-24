using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector.Lifestyles;
using SimpleInjector;
using CleanArchitecture.Infrastructure.Testing;
using CleanArchitecture.Domain.Repositories;

namespace CleanArchitecture.UnitTests.Repositories
{
    [TestClass]
    public class UsersRepositoryTests : TestBase
    {
        private static Container _container;
        private static Scope _containerScope;

        private static IUsersRepository _usersRepository;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // build configuration            
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"))
                .AddJsonFile("test.settings.json", optional: false, reloadOnChange: false);
            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection()
                .AddSimpleInjector(_container);

            services
                .BuildServiceProvider(validateScopes: true)
                .UseSimpleInjector(_container);

            //_container.Register(() => new ServiceFactory(_container.GetInstance), Lifestyle.Singleton);

            _container.Verify();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            _container.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _containerScope = AsyncScopedLifestyle.BeginScope(_container);
            //_usersRepository = _container.GetService<IUsersRepository>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _containerScope.Dispose();
        }
    }
}
