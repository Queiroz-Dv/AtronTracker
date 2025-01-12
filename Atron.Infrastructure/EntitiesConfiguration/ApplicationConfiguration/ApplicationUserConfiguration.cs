using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Models.ApplicationModels;

namespace Atron.Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    // Reconfiguração da classe User do Identity    
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AppUser");

            builder.HasKey(x => x.Id);

            builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

            builder.Property(u => u.UserName).HasMaxLength(50).IsRequired();
            builder.Property(u => u.NormalizedUserName).HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(25);
            builder.Property(u => u.NormalizedEmail).HasMaxLength(25);
            builder.Property(u => u.PhoneNumberConfirmed);
            builder.Property(u => u.TwoFactorEnabled);
            builder.Property(u => u.LockoutEnd);
            builder.Property(u => u.LockoutEnabled);
            builder.Property(u => u.AccessFailedCount);

            builder.Property(u => u.PasswordHash).HasMaxLength(450).IsRequired(false);
            builder.Property(u => u.PhoneNumber).HasMaxLength(25).IsRequired(false);
            // Each User can have many UserClaims
            builder.HasMany<ApplicationUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

            // Each User can have many UserLogins
            builder.HasMany<ApplicationUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

            // Each User can have many UserTokens
            builder.HasMany<ApplicationUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany<ApplicationUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        }
    }
}