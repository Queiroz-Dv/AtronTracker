using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Codigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CPF).IsRequired(false)
                .HasMaxLength(11);

            builder.Property(c => c.CNPJ).IsRequired(false)
                .HasMaxLength(14);          

            builder.Property(c => c.AuditoriaId).IsRequired(false);

            builder.OwnsOne(c => c.Endereco, e =>
            {
                e.Property(ed => ed.Logradouro).IsRequired(false).HasMaxLength(100);
                e.Property(ed => ed.Numero).IsRequired(false).HasMaxLength(10);
                e.Property(ed => ed.Cidade).IsRequired(false).HasMaxLength(50);
                e.Property(ed => ed.UF).IsRequired(false).HasMaxLength(2);
                e.Property(ed => ed.CEP).IsRequired(false).HasMaxLength(9);
            });

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Telefone)
                .IsRequired()
                .HasMaxLength(15);
        }
    }
}