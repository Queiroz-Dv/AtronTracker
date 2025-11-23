using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class UsuarioCargoDepartamentoConfiguration : IEntityTypeConfiguration<UsuarioCargoDepartamento>
    {
        public void Configure(EntityTypeBuilder<UsuarioCargoDepartamento> builder)
        {
            builder.ToTable("UsuarioCargoDepartamento");
            builder.HasKey(ucd => new { 
                ucd.UsuarioId, 
                ucd.UsuarioCodigo,
                ucd.CargoId, 
                ucd.CargoCodigo, 
                ucd.DepartamentoId, 
                ucd.DepartamentoCodigo });

            builder.HasOne(ucd => ucd.Usuario)
                   .WithMany(usr => usr.UsuarioCargoDepartamentos)
                   .HasForeignKey(ucd => new { ucd.UsuarioId, ucd.UsuarioCodigo })
                   .HasPrincipalKey(usr => new { usr.Id, usr.Codigo });

            builder.HasOne(ucd => ucd.Cargo)
                   .WithMany(crg => crg.UsuarioCargoDepartamentos)
                   .HasForeignKey(ucd => new { ucd.CargoId, ucd.CargoCodigo })
                   .HasPrincipalKey(crg => new { crg.Id, crg.Codigo });

            builder.HasOne(ucd => ucd.Departamento)
                   .WithMany(dpt => dpt.UsuarioCargoDepartamentos)
                   .HasForeignKey(ucd => new { ucd.DepartamentoId, ucd.DepartamentoCodigo })
                   .HasPrincipalKey(dpt => new { dpt.Id, dpt.Codigo });
        }
    }
}