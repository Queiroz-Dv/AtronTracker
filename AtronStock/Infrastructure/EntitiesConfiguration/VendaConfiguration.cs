using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class VendaConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.ClienteCodigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(v => v.DataVenda)
                .IsRequired();

            builder.HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Itens)
                .WithOne(i => i.Venda)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
