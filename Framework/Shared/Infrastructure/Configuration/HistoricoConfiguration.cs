using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Configuration
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