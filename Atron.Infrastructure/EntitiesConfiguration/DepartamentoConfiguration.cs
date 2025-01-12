using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.HasKey(dpt => new { dpt.Id, dpt.Codigo });
            builder.Property(dpt => dpt.Id).ValueGeneratedOnAdd();

            builder.Property(dpt => dpt.IdSequencial).IsRequired();

            builder.Property(dpt => dpt.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}