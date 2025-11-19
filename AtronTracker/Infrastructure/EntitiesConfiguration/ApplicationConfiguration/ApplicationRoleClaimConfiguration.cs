using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities.Identity;

namespace Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    public class ApplicationRoleClaimConfiguration : IEntityTypeConfiguration<ApplicationRoleClaim>
    {
        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.ToTable("AppRoleClaim");

            builder.HasKey(x => x.Id);
            builder.Property(rlc => rlc.ClaimType).HasMaxLength(128).IsRequired(false);
            builder.Property(rlc => rlc.ClaimValue).HasMaxLength(128).IsRequired(false);
        }
    }
}