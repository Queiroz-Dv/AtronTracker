using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class PlanejamentoCustoConfiguration : IEntityTypeConfiguration<PlanejamentoCusto>
    {
        public void Configure(EntityTypeBuilder<PlanejamentoCusto> builder)
        {
            builder.HasKey(plc => new { plc.Id, plc.Codigo });
            builder.Property(plc => plc.Id).ValueGeneratedOnAdd();

            builder.Property(plc => plc.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(plc => plc.Descricao)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(plc => plc.ValorMinimo)
                .HasPrecision(18, 2);

            builder.Property(plc => plc.ValorTeto)
                .HasPrecision(18, 2);

            builder.Property(plc => plc.DepartamentoCodigo)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasOne(plc => plc.Departamento)
                .WithMany(dpt => dpt.PlanejamentosCusto)
                .HasForeignKey(plc => new { plc.DepartamentoId, plc.DepartamentoCodigo })
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(plc => new { plc.DepartamentoId, plc.DepartamentoCodigo, plc.Ano })
                .IsUnique();
        }
    }
}
