using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(usr => new { usr.Id, usr.Codigo });
            builder.Property(usr => usr.Id).ValueGeneratedOnAdd();

            builder.Property(usr => usr.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(usr => usr.Nome).IsRequired().HasMaxLength(25);
            builder.Property(usr => usr.Sobrenome).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.Email).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.DataNascimento);
            builder.Property(usr => usr.SalarioAtual);

            builder.HasMany(pfa => pfa.PerfisDeAcesso)
                   .WithMany(usr => usr.Usuarios);                  
        }
    }
}