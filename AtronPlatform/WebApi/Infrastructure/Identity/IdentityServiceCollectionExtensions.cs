using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Entities.Identity;
using Shared.Infrastructure.Configuration;
using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace AtronPlatform.WebApi.Infrastructure.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddAtronIdentity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var database = DatabaseProviderResolver.Resolve(configuration);
            var migrationsAssembly = typeof(AtronIdentityDbContext).Assembly.GetName().Name!;

            services.AddDbContext<AtronIdentityDbContext>(options =>
                options.UseConfiguredDatabase(database, migrationsAssembly)
            );

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<AtronIdentityDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            services.AddDataProtection()
                .SetApplicationName("Atron")
                .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            return services;
        }
    }
}
