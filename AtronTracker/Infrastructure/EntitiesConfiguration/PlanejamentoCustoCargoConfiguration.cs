using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class PlanejamentoCustoCargoConfiguration : IEntityTypeConfiguration<PlanejamentoCustoCargo>
    {
        public void Configure(EntityTypeBuilder<PlanejamentoCustoCargo> builder)
        {
            builder.HasKey(pcc => pcc.Id);
            builder.Property(pcc => pcc.Id).ValueGeneratedOnAdd();

            builder.Property(pcc => pcc.PlanejamentoCustoCodigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(pcc => pcc.CargoCodigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(pcc => pcc.ValorMinimo)
                .HasPrecision(18, 2);

            builder.Property(pcc => pcc.ValorTeto)
                .HasPrecision(18, 2);

            builder.HasOne(pcc => pcc.PlanejamentoCusto)
                .WithMany(plc => plc.DetalhesCargo)
                .HasForeignKey(pcc => new { pcc.PlanejamentoCustoId, pcc.PlanejamentoCustoCodigo })
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pcc => pcc.Cargo)
                .WithMany(crg => crg.PlanejamentosCustoCargo)
                .HasForeignKey(pcc => new { pcc.CargoId, pcc.CargoCodigo })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pcc => new { pcc.PlanejamentoCustoId, pcc.PlanejamentoCustoCodigo, pcc.CargoId, pcc.CargoCodigo })
                .IsUnique();
        }
    }
}
