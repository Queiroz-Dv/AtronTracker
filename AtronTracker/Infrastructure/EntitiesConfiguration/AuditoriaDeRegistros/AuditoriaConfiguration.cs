using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration.AuditoriaDeRegistros
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.DataCriacao)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.DataAlteracao).IsRequired(false);

            builder.Property(a => a.CriadoPor)
                   .IsRequired()
                   .HasMaxLength(25);

            builder.Property(a => a.AlteradoPor)
                     .IsRequired(false)
                     .HasMaxLength(25);

            builder.Property(a => a.ModuloCodigo)
                     .IsRequired()
                     .HasMaxLength(10);

            builder.Property(a => a.Inativo)
                     .IsRequired(false)
                     .HasDefaultValue(false);

            builder.HasMany(a => a.Historicos)
                   .WithOne(h => h.Auditoria)
                   .HasForeignKey(h => h.AuditoriaId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}