using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class MembroWorkspaceConfiguration : IEntityTypeConfiguration<MembroWorkspace>
    {
        public void Configure(EntityTypeBuilder<MembroWorkspace> builder)
        {
            builder.HasKey(workspace => workspace.Id);
            builder.Property(workspace => workspace.Id).ValueGeneratedOnAdd();

            builder.Property(work => work.UsuarioCodigo).IsRequired().HasMaxLength(10);
            builder.Property(work => work.WorkspaceCodigo).IsRequired().HasMaxLength(10);

            builder.HasOne(work => work.Workspace)
                   .WithMany(membros => membros.Membros)
                   .HasForeignKey(fk => new {fk.WorkspaceId, fk.WorkspaceCodigo});
        }
    }
}