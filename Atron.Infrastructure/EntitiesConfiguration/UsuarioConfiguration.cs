using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atron.Infrastructure.EntitiesConfiguration
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(usr => usr.Id).ValueGeneratedNever();

            builder.Property(usr => usr.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(usr => usr.Nome).IsRequired().HasMaxLength(25);
            builder.Property(usr => usr.Sobrenome).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.DataNascimento);
            builder.Property(usr => usr.Salario);
            builder.Property(usr => usr.CargoId).IsRequired();
            builder.Property(usr => usr.DepartamentoId).IsRequired();
            builder.Property(usr => usr.CargoCodigo).IsRequired().HasMaxLength(10);
            builder.Property(usr => usr.DepartamentoCodigo).IsRequired().HasMaxLength(10);
        }
    }
}