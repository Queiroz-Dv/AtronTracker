using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

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

            builder.Property(c => c.Status)
                   .HasDefaultValue(EStatus.Ativo)
                   .HasMaxLength(80)
                   .HasConversion(
                      tipo => tipo.GetDescription(),
                      descricao => ParseTipoStatus(descricao)
                   ).IsRequired();
        }

        private static EStatus ParseTipoStatus(string descricao)
        {
            foreach (EStatus tipo in Enum.GetValues(typeof(EStatus)))
            {
                if (tipo.GetDescription().Equals(descricao, StringComparison.OrdinalIgnoreCase) ||
                    tipo.ToString().Equals(descricao, StringComparison.OrdinalIgnoreCase))
                {
                    return tipo;
                }
            }

            return EStatus.Ativo;
        }
    }
}