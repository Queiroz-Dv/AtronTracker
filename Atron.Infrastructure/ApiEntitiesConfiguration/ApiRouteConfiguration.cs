using Atron.Domain.ApiEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.ApiEntitiesConfiguration
{
    internal class ApiRouteConfiguration : IEntityTypeConfiguration<ApiRoute>
    {
        public void Configure(EntityTypeBuilder<ApiRoute> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(apr => apr.ModuleName).IsRequired().HasMaxLength(60);
            builder.Property(apr => apr.RouteUrl).IsRequired().HasMaxLength(100);
            builder.Property(apr => apr.HttpMethod).IsRequired().HasMaxLength(7);
            builder.Property(apr => apr.IsActive).IsRequired();
        }
    }
}