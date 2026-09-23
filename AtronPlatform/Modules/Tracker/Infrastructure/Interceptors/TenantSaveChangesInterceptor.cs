using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Infrastructure.Interceptors
{
    public class TenantSaveChangesInterceptor : SaveChangesInterceptor
    {

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            // The interceptor receives the context via the event data.
            var context = (AtronDbContext)eventData.Context;

            // Capture tenant‑scoped entities that are being added.
            var entries = context.ChangeTracker
                .Entries<ITenantScoped>()
                .Where(e => e.State == EntityState.Added)
                .ToList();

            if (entries.Count == 0)
                return await base.SaveChangesAsync(cancellationToken);

            var workspaceCodigo = context.WorkspaceCodigoAtual;
            if (string.IsNullOrEmpty(workspaceCodigo))
                return await base.SaveChangesAsync(cancellationToken);

            var workspaceId = await context.Workspaces
                .Where(w => w.Codigo == workspaceCodigo)
                .Select(w => w.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (workspaceId == 0)
                return await base.SaveChangesAsync(cancellationToken);

            // Execute base save within a transaction so we can capture generated Ids
            var executionStrategy = context.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                var result = await base.SaveChangesAsync(cancellationToken);

                foreach (var entry in entries)
                {
                    context.RequisitosWorkspace.Add(new RecursoWorkspace
                    {
                        WorkspaceId = workspaceId,
                        WorkspaceCodigo = workspaceCodigo,
                        ModuloCodigo = entry.Entity.ModuloCodigo,
                        RecursoId = entry.Entity.Id,
                        RecursoCodigo = entry.Entity.Codigo
                    });
                }

                await base.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return result;
            });
        }

        public void ApplyTenantFilters(ModelBuilder modelBuilder)
        {
            var tenantScopedEntities = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(ITenantScoped).IsAssignableFrom(e.ClrType));

            foreach (var entityType in tenantScopedEntities)
            {
                var method = typeof(AtronDbContext)
                    .GetMethod(nameof(ConfigureTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }

        private void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class, ITenantScoped
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
                RecursosWorkspace.Any(rw =>
                    rw.RecursoId == e.Id &&
                    rw.ModuloCodigo == e.ModuloCodigo &&
                    rw.WorkspaceCodigo == WorkspaceCodigoAtual));
        }
    }
}
