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
            builder.Property(apr => apr.Modulo).IsRequired().HasMaxLength(60);
            builder.Property(apr => apr.Acao).IsRequired();
            builder.Property(apr => apr.Ativo).IsRequired();
            builder.Property(apr => apr.NomeDaRotaDeAcesso).IsRequired().HasMaxLength(30);
        }
    }
}