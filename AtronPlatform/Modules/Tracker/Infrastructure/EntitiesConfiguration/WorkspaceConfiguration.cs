using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.HasKey(workspace => new {workspace.Id, workspace.Codigo});
            builder.Property(workspace => workspace.Id).ValueGeneratedOnAdd();
            builder.Property(workspace => workspace.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(workspace => workspace.Descricao).IsRequired().HasMaxLength(150);
            builder.Property(workspace => workspace.ResponsavelEmail).IsRequired().HasMaxLength(50);

            builder.HasOne(workspace => workspace.Responsavel)
                   .WithOne(u => u.Workspace)
                   .HasForeignKey<Workspace>(workspace => new { workspace.ResponsavelId, workspace.ResponsavelCodigo})
                   .HasPrincipalKey<Usuario>(usuario => new { usuario.Id, usuario.Codigo })
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}