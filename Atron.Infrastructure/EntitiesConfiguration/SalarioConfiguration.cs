using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class SalarioConfiguration : IEntityTypeConfiguration<Salario>
    {
        public void Configure(EntityTypeBuilder<Salario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(slr => slr.UsuarioId).IsRequired();
            builder.Property(slr => slr.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(slr => slr.MesId).IsRequired();
            builder.Property(slr => slr.QuantidadeTotal).IsRequired();
            builder.Property(slr => slr.Ano).IsRequired();
        }
    }
}