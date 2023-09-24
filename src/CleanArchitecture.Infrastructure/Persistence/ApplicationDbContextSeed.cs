using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDataAsync(IApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Name = "Siyanda",
                        Surname = "Mazibuko",
                        Telephone = "1234567896",
                        EmailAddress ="sia@gmial.com",
                        DateOfBirth = DateTime.Now
                    }
                );
            }

            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
