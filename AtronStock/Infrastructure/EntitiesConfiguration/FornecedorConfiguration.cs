using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.ToTable("Fornecedores");

            builder.HasKey(f => f.Id);

            builder.HasIndex(f => f.Codigo).IsUnique();

            builder.Property(f => f.Codigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.CNPJ)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(f => f.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Telefone)
                .IsRequired()
                .HasMaxLength(20);

            builder.OwnsOne(f => f.Endereco, e =>
            {
                e.Property(ed => ed.Logradouro).IsRequired(false).HasMaxLength(100);
                e.Property(ed => ed.Numero).IsRequired(false).HasMaxLength(10);
                e.Property(ed => ed.Cidade).IsRequired(false).HasMaxLength(50);
                e.Property(ed => ed.UF).IsRequired(false).HasMaxLength(2);
                e.Property(ed => ed.CEP).IsRequired(false).HasMaxLength(9);
            });
        }
    }
}
