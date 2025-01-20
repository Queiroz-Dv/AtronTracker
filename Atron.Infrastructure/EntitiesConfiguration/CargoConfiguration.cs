using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            builder.HasKey(crg => new { crg.Id, crg.Codigo });

            builder.Property(ppt => ppt.Id).ValueGeneratedOnAdd();

            builder.Property(pst => pst.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(dpt => dpt.Departamento) // Tem um departamento
                   .WithMany(crg => crg.Cargos) // com muitos cargos 
                   .HasForeignKey(key => new { key.DepartmentoId, key.DepartamentoCodigo }) // FK da relação
                   .HasPrincipalKey(dpt => new { dpt.Id, dpt.Codigo });

            // Exemplo pra preencher a tabela 
            //builder.HasData(new Position(1, "Mkrt Manager"));
        }
    }
}