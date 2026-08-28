using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(empresa => empresa.Id);
            builder.Property(empresa => empresa.Codigo).IsRequired().HasMaxLength(25);
            builder.Property(empresa => empresa.NomeFantasia).IsRequired().HasMaxLength(150);
            builder.OwnsOne(empresa => empresa.Endereco, endereco =>
            {
                endereco.Property(item => item.Logradouro)
                    .HasColumnName("Endereco").IsRequired().HasMaxLength(200);
            });
            builder.Navigation(empresa => empresa.Endereco).IsRequired();
            builder.Property(empresa => empresa.Numero).IsRequired().HasMaxLength(20);
            builder.Property(empresa => empresa.Email).IsRequired().HasMaxLength(254);
            builder.Property(empresa => empresa.Status).IsRequired();
            builder.HasIndex(empresa => empresa.Codigo).IsUnique();
            builder.Property(empresa => empresa.Status).IsConcurrencyToken();
        }
    }
}
