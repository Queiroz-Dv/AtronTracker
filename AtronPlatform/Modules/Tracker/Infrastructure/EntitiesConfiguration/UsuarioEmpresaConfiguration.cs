using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class UsuarioEmpresaConfiguration : IEntityTypeConfiguration<UsuarioEmpresa>
    {
        public void Configure(EntityTypeBuilder<UsuarioEmpresa> builder)
        {
            builder.HasKey(vinculo => vinculo.Id);
            builder.Property(vinculo => vinculo.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(vinculo => vinculo.Papel).IsRequired();
            builder.Property(vinculo => vinculo.Status).IsRequired();

            builder.HasIndex(vinculo => new { vinculo.UsuarioId, vinculo.UsuarioCodigo }).IsUnique();

            builder.HasOne(vinculo => vinculo.Empresa)
                .WithMany(empresa => empresa.Usuarios)
                .HasForeignKey(vinculo => vinculo.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vinculo => vinculo.Usuario)
                .WithMany()
                .HasForeignKey(vinculo => new { vinculo.UsuarioId, vinculo.UsuarioCodigo })
                .HasPrincipalKey(usuario => new { usuario.Id, usuario.Codigo })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
