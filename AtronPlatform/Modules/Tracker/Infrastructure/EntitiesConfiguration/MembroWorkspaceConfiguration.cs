using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class MembroWorkspaceConfiguration : IEntityTypeConfiguration<MembroWorkspace>
    {
        public void Configure(EntityTypeBuilder<MembroWorkspace> builder)
        {
            builder.ToTable("MembrosWorkspace");
            builder.HasKey(membro => new { membro.WorkspaceId, membro.UsuarioCodigo });
            builder.Property(membro => membro.UsuarioCodigo).IsRequired().HasMaxLength(10);

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
