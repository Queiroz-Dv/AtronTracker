using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration
{
    public class RecursoWorkspaceConfiguration : IEntityTypeConfiguration<RecursoWorkspace>
    {
        public void Configure(EntityTypeBuilder<RecursoWorkspace> builder)
        {
            builder.ToTable("RecursoWorkspace");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.RecursoId, x.ModuloCodigo, x.WorkspaceId })
                   .HasDatabaseName("ix_recurso_workspace_pesquisa");
                   
            builder.HasOne(x => x.Workspace)
                   .WithMany()
                   .HasForeignKey(x => new { x.WorkspaceId, x.WorkspaceCodigo })
                   .HasPrincipalKey(w => new { w.Id, w.Codigo })
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
