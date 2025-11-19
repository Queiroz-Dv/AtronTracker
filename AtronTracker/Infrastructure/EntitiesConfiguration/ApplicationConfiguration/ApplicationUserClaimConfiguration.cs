using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities.Identity;

namespace Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    public class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
        {
            builder.ToTable("AppUserClaim");

            builder.HasKey(pk => pk.Id);
            builder.Property(uc => uc.ClaimType).IsRequired(false).HasMaxLength(50);
            builder.Property(uc => uc.ClaimValue).IsRequired(false).HasMaxLength(50);
        }
    }
}