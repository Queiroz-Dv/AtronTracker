using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class PerfilDeAcessoConfiguration : IEntityTypeConfiguration<PerfilDeAcesso>
    {
        public void Configure(EntityTypeBuilder<PerfilDeAcesso> builder)
        {
            builder.HasKey(pfa => new { pfa.Id, pfa.Codigo });

            builder.Property(pfa => pfa.Id).ValueGeneratedOnAdd();

            builder.Property(pfa => pfa.Codigo).IsRequired().HasMaxLength(10);

            builder.Property(pfa => pfa.Descricao).IsRequired().HasMaxLength(50);
        }
    }
}