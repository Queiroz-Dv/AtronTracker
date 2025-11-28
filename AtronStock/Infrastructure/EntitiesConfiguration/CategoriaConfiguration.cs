using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.HasAlternateKey(c => c.Codigo);

            builder.Property(c => c.Codigo)
                   .IsRequired()
                   .HasMaxLength(25);

            builder.Property(c => c.Descricao)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Status).IsRequired();
        }
    }
}