using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.Identity;

namespace AtronPlatform.WebApi.Infrastructure.Identity
{
    public class AtronIdentityDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>
    {
        public AtronIdentityDbContext(DbContextOptions<AtronIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("AppUser");
            builder.Entity<ApplicationRole>().ToTable("AppRole");
            builder.Entity<ApplicationUserClaim>().ToTable("AppUserClaim");
            builder.Entity<ApplicationUserRole>().ToTable("AppUserRole");
            builder.Entity<ApplicationUserLogin>().ToTable("AppUserLogin");
            builder.Entity<ApplicationRoleClaim>().ToTable("AppRoleClaim");
            builder.Entity<ApplicationUserToken>().ToTable("AppUserToken");

            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                {
                    var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
                    if (propertyType == typeof(DateTime))
                        property.SetColumnType("timestamp without time zone");
                }
            }
        }
    }
}
