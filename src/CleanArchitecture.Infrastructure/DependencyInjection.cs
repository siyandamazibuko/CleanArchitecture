using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Common.Data;
using CleanArchitecture.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Application.Mappings;
using CleanArchitecture.Application.Handlers.Users;
using CleanArchitecture.Application.Validators.Users;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(UserProfileMapper).Assembly);
            services.AddValidatorsFromAssembly(typeof(CreateUserCommandValidator).Assembly);
            services.AddMediatR(typeof(GetUsersQueryHandler).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services;
        }
        
        public static IServiceCollection AddIntegrations(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services.AddTransient<IUsersRepository, UsersRepository>();

            return services;
        }
        
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationInMemoryDbContext>(options =>
                    options.UseInMemoryDatabase("clean.architecture"));
                
                services.AddScoped<DbContext, ApplicationInMemoryDbContext>();
                services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationInMemoryDbContext>());
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options
                    .UseLazyLoadingProxies()
                    .UseNpgsql(configuration.GetConnectionString("CleanArchitectureDbConnection"),
                        a => a.MigrationsAssembly("CleanArchitecture.Tools.Postgres"))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging());

                services.AddScoped<DbContext, ApplicationDbContext>();
                services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());              
            }

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddAutoMapper(typeof(Infrastructure.Mappings.UserProfileMapper).Assembly);

            
            return services;
        }
    }
}
