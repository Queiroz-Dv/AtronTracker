using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class ConfirmacaoEmailConfiguration : IEntityTypeConfiguration<ConfirmacaoEmail>
    {
        public void Configure(EntityTypeBuilder<ConfirmacaoEmail> builder)
        {
            builder.HasKey(cfm => cfm.Id);

            builder.Property(cfm => cfm.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(cfm => cfm.IdentificadorHash).IsRequired().HasMaxLength(128);
            builder.Property(cfm => cfm.CriadoEm).IsRequired();
            builder.Property(cfm => cfm.ExpiraEm).IsRequired();
            builder.Property(cfm => cfm.ConfirmadoEm).IsRequired(false);

            builder.HasIndex(cfm => new { cfm.UsuarioCodigo, cfm.ExpiraEm, cfm.ConfirmadoEm });
        }
    }
}
