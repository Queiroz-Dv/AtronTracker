using AtronStock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtronStock.Infrastructure.EntitiesConfiguration
{
    public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
    {
        public void Configure(EntityTypeBuilder<ItemVenda> builder)
        {
            builder.ToTable("ItensVenda");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProdutoCodigo)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(i => i.Quantidade)
                .IsRequired();

            builder.Property(i => i.PrecoVenda)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
