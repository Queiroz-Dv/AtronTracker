using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Models.ApplicationModels;

namespace Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            builder.ToTable("AppUserToken");

            builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(t => t.Name).HasMaxLength(50).IsRequired();
            builder.Property(t => t.LoginProvider).HasMaxLength(128).IsRequired();
            builder.Property(t => t.Value).HasMaxLength(450).IsRequired(false);
        }
    }
}