using System;
using AutoMapper;
using CleanArchitecture.Infrastructure.Mappings;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Testing.Factories;

namespace CleanArchitecture.Infrastructure.Testing
{
    public class UnitTestBase : TestBase, IDisposable
    {
        private bool _disposed;
        protected ApplicationInMemoryDbContext Context { get; }
        protected IMapper Mapper { get; }

        protected UnitTestBase()
        {
            Context = InMemoryDbContextFactory.Create();

            var configurationProvider = new MapperConfiguration(cfg =>
            {   
                cfg.AddMaps(typeof(UserProfileMapper).Assembly);
                cfg.AddMaps(typeof(Application.Mappings.UserProfileMapper).Assembly);
            });
            
            Mapper = configurationProvider.CreateMapper();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                InMemoryDbContextFactory.Destroy(Context);
            }
            _disposed = true;
        }
    }
}
