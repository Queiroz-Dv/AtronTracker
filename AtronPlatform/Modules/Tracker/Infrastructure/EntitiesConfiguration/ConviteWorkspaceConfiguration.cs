using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration;

public sealed class ConviteWorkspaceConfiguration
    : IEntityTypeConfiguration<ConviteWorkspace>
{
    public void Configure(EntityTypeBuilder<ConviteWorkspace> builder)
    {
        builder.ToTable("ConvitesWorkspace");
        builder.HasKey(convite => convite.Id);
        builder.Property(convite => convite.Id).ValueGeneratedOnAdd();
        builder.Property(convite => convite.IdentificadorHash)
            .IsRequired()
            .HasMaxLength(64);
        builder.Property(convite => convite.RemetenteCodigo)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(convite => convite.ExpiraEm).IsRequired();
        builder.Property(convite => convite.UtilizadoPorUsuarioCodigo)
            .HasMaxLength(10)
            .IsRequired(false);
        builder.Property(convite => convite.UtilizadoEm).IsRequired(false);

        builder.HasIndex(convite => convite.IdentificadorHash).IsUnique();
        builder.HasIndex(convite => new
        {
            convite.WorkspaceId,
            convite.ExpiraEm,
            convite.UtilizadoEm
        });

        builder.HasOne(convite => convite.Workspace)
            .WithMany()
            .HasForeignKey(convite => convite.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(convite => convite.Remetente)
            .WithMany()
            .HasForeignKey(convite => convite.RemetenteCodigo)
            .HasPrincipalKey(usuario => usuario.Codigo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(convite => convite.UtilizadoPorUsuario)
            .WithMany()
            .HasForeignKey(convite => convite.UtilizadoPorUsuarioCodigo)
            .HasPrincipalKey(usuario => usuario.Codigo)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
