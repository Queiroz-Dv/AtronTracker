using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.HasKey(empresa => empresa.Id);
            builder.Property(empresa => empresa.Id).ValueGeneratedOnAdd();
            builder.Property(empresa => empresa.Codigo).IsRequired().HasMaxLength(25);
            builder.Property(empresa => empresa.NomeFantasia).IsRequired().HasMaxLength(150);
            builder.Property(empresa => empresa.Endereco).IsRequired().HasMaxLength(200);
            builder.Property(empresa => empresa.Numero).IsRequired().HasMaxLength(20);
            builder.Property(empresa => empresa.Email).IsRequired().HasMaxLength(254);
            builder.Property(empresa => empresa.Status)
                .HasConversion(EnumStringConverter.Create<Domain.Enums.StatusEmpresa>())
                .HasMaxLength(30)
                .IsRequired();
            builder.HasAlternateKey(empresa => empresa.Codigo);
        }
    }
}
