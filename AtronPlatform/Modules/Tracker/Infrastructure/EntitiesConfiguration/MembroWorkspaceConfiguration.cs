using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class MembroWorkspaceConfiguration : IEntityTypeConfiguration<MembroWorkspace>
    {
        public void Configure(EntityTypeBuilder<MembroWorkspace> builder)
        {
            builder.ToTable("Membros_Workspace");
            builder.HasKey(membro => new { membro.WorkspaceId, membro.UsuarioCodigo });
            builder.Property(membro => membro.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(membro => membro.Tipo)
                   .HasConversion(EnumStringConverter.Create<TipoMembroWorkspace>())
                   .HasMaxLength(30)
                   .IsRequired();

            builder.HasOne(membro => membro.Workspace)
                   .WithMany(workspace => workspace.Membros)
                   .HasForeignKey(membro => membro.WorkspaceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(membro => membro.Usuario)
                   .WithMany()
                   .HasForeignKey(membro => membro.UsuarioCodigo)
                   .HasPrincipalKey(usuario => usuario.Codigo)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
