using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Configuration
{
    public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.HasIndex(a => a.CodigoRegistro);
            builder.Property(a => a.CodigoRegistro)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.DataCriacao)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");


            builder.Property(a => a.CriadoPor)
                   .IsRequired()
                   .HasMaxLength(25);

            builder.Property(a => a.AlteradoPor)
                     .IsRequired(false)
                     .HasMaxLength(25);

            builder.Property(a => a.DataAlteracao).IsRequired(false);

            builder.Property(a => a.RemovidoEm)
                     .IsRequired(false);
        }
    }
}