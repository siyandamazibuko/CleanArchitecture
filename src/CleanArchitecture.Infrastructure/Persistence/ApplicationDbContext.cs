using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Configuration;
using CleanArchitecture.Infrastructure.Persistence.Extensions;
using System;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {               

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public bool IsInMemoryDb
        {
            get { return this.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";   }
        }

        public virtual DbSet<User> Users { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplySingularTableNames();
            modelBuilder.ApplySnakeCaseNames();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserMap).Assembly);
        }
       
    }
}
