using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Models.ApplicationModels;

namespace Atron.Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    public class ApplicationUserLoginConfiguration : IEntityTypeConfiguration<ApplicationUserLogin>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            builder.ToTable("AppUserLogin");

            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            // Limit the size of the composite key columns due to common DB restrictions
            builder.Property(l => l.LoginProvider).HasMaxLength(128);
            builder.Property(l => l.ProviderKey).HasMaxLength(128);
            
            // Mapeamento não necessário
            builder.Ignore(l => l.ProviderDisplayName);
        }
    }
}
