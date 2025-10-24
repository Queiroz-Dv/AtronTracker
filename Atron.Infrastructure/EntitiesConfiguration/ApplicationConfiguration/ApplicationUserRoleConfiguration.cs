using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Models.ApplicationModels;

namespace Atron.Tracker.Infrastructure.EntitiesConfiguration.ApplicationConfiguration
{
    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.ToTable("AppUserRole");

            builder.HasKey(e => new { e.UserId, e.RoleId });
        }
    }
}