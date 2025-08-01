using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class SalarioConfiguration : IEntityTypeConfiguration<Salario>
    {
        public void Configure(EntityTypeBuilder<Salario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(slr => slr.SalarioMensal).IsRequired();
            builder.Property(slr => slr.Ano).HasMaxLength(4).IsRequired();
            builder.Property(slr => slr.MesId).HasMaxLength(12).IsRequired();    

            //builder.HasOne(slr => slr.Usuario)
            //       .WithOne(usr => usr.Salario)
            //       .HasForeignKey<Salario>(slr => new { slr.UsuarioId, slr.UsuarioCodigo})
            //       .IsRequired();        
        }
    }
}