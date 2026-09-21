using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfiguration.TenantsConfiguration
{
    public class DepartamentoWorkspaceTenant : IEntityTypeConfiguration<DepartamentoWorkspace>
    {
        public void Configure(EntityTypeBuilder<DepartamentoWorkspace> builder)
        {
            builder.ToTable("DepartamentoTenant");
            builder.HasKey(k => new
            {
                k.WorkspaceId,
                k.WorkspaceCodigo,
                k.DepartamentoId,
                k.DepartamentoCodigo
            });
        }
    }
}
