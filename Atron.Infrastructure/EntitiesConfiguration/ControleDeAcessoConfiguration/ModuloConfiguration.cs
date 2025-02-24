using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration.ControleDeAcessoConfiguration
{
    public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
    {
        public void Configure(EntityTypeBuilder<Modulo> builder)
        {
            builder.HasKey(mod => new { mod.Id, mod.Codigo });
            builder.Property(mod => mod.Id).ValueGeneratedOnAdd();

            builder.Property(mod => mod.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(mod => mod.Descricao).IsRequired().HasMaxLength(100);

            builder.HasMany(mod => mod.PerfilDeAcessoModulos)
                   .WithOne(pam => pam.Modulo)
                   .HasForeignKey(pam => new { pam.ModuloId, pam.ModuloCodigo });

            builder.HasMany(mod => mod.PropriedadeDeFluxoModulos)
                   .WithOne(pfm => pfm.Modulo)
                   .HasForeignKey(pfm => new { pfm.ModuloId, pfm.ModuloCodigo });
        }
    }
}