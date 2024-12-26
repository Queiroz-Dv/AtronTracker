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

            builder.Property(dpt => dpt.IdSequencial).IsRequired();

            builder.Property(usr => usr.Nome).IsRequired().HasMaxLength(25);
            builder.Property(usr => usr.Sobrenome).IsRequired().HasMaxLength(50);
            builder.Property(usr => usr.DataNascimento);
            builder.Property(usr => usr.Salario);

            builder.HasOne(crg => crg.Cargo)
                   .WithMany(usr => usr.Usuarios)
                   .HasForeignKey(key => new { key.CargoId, key.CargoCodigo })
                   .HasPrincipalKey(crg => new { crg.Id, crg.Codigo });

            builder.HasOne(crg => crg.Departamento)
                   .WithMany(usr => usr.Usuarios)
                   .HasForeignKey(key => new { key.DepartamentoId, key.DepartamentoCodigo })
                   .HasPrincipalKey(dpt => new { dpt.Id, dpt.Codigo });
        }
    }
}