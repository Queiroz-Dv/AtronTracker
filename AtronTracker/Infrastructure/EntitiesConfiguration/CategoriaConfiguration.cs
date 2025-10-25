using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.HasAlternateKey(c => c.Codigo);
            builder.HasIndex(c => c.Codigo)
                   .IsUnique()
                   .HasDatabaseName("IX_Categoria_Codigo");

            builder.Property(c => c.Codigo)
                   .IsRequired()
                   .HasMaxLength(25);

            builder.Property(c => c.Removido)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(c => c.RemovidoEm).IsRequired(false);

            builder.Property(c => c.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}