using System;
using Microsoft.EntityFrameworkCore;
using Moq;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;

namespace CleanArchitecture.Infrastructure.Testing.Factories
{
    public class InMemoryDbContextFactory
    {
        public static ApplicationInMemoryDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationInMemoryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            var context = new ApplicationInMemoryDbContext(options);

            context.Database.EnsureCreated();

            context.SaveChanges();

            return context;
        }

        public static void Destroy(ApplicationInMemoryDbContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}
