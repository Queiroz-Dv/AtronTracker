using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Extensions;

namespace Infrastructure.EntitiesConfiguration
{
    public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.HasKey(workspace => workspace.Id);
            builder.Property(workspace => workspace.Id).ValueGeneratedOnAdd();
            builder.Property(workspace => workspace.Nome).IsRequired().HasMaxLength(150);
            builder.Property(workspace => workspace.Tipo)
                .HasConversion(EnumStringConverter.Create<Domain.Enums.TipoWorkspace>())
                .HasMaxLength(30)
                .IsRequired();
            builder.Property(workspace => workspace.EmpresaCodigo).HasMaxLength(25);

            builder.HasOne(workspace => workspace.Empresa)
                   .WithOne()
                   .HasForeignKey<Workspace>(workspace => workspace.EmpresaCodigo)
                   .HasPrincipalKey<Empresa>(empresa => empresa.Codigo)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);
        }
    }
}
