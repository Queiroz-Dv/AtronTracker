using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration.AuditoriaDeRegistros
{
    public class HistoricoConfiguration : IEntityTypeConfiguration<Historico>
    {
        public void Configure(EntityTypeBuilder<Historico> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id).ValueGeneratedOnAdd();

            builder.Property(h => h.AuditoriaId)
                   .IsRequired();

            builder.Property(h => h.Descricao)
                   .IsRequired()
                   .HasMaxLength(1500);
        }
    }
}