using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.HasKey(dpt => dpt.Id);

            builder.Property(dpt => dpt.Id)
                   .ValueGeneratedNever();

            builder.Property(dpt => dpt.Codigo)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(dpt => dpt.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}