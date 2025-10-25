using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
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

            builder.Property(c => c.CPF)
                .HasMaxLength(11);

            builder.Property(c => c.CNPJ)
                .HasMaxLength(14);

            builder.Property(c => c.Status)
                .IsRequired();

            builder.OwnsOne(c => c.Endereco, e =>
            {
                e.Property(ed => ed.Logradouro).HasMaxLength(100);
                e.Property(ed => ed.Numero).HasMaxLength(10);
                e.Property(ed => ed.Cidade).HasMaxLength(50);
                e.Property(ed => ed.UF).HasMaxLength(2);
                e.Property(ed => ed.CEP).HasMaxLength(9);
            });

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Telefone)
                .IsRequired()
                .HasMaxLength(15);

            builder.HasMany(c => c.Vendas)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId);         
        }
    }
}