using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Application.DTOS.Auth;
using Shared.Domain.Entities.Identity;
using System.Reflection;

namespace Infrastructure.Context
{
    public class AtronDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public bool IsProcessingTenantRecords { get; set; }

        // Propriedade auxiliar para obter o código do workspace atual
        public string CurrentWorkspaceCodigo => _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimCode.CODIGO_WORKSPACE)?.Value ?? string.Empty;

        public AtronDbContext(DbContextOptions<AtronDbContext> options, IHttpContextAccessor httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<MembroWorkspace> MembrosWorkspace { get; set; }
        public DbSet<ConfirmacaoEmail> ConfirmacoesEmail { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<TarefaMovimentacao> TarefaMovimentacoes { get; set; }
        public DbSet<SolicitacaoObtencaoTarefa> SolicitacoesObtencaoTarefa { get; set; }
        public DbSet<TarefaEstado> TarefaEstados { get; set; }
        public DbSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
        public DbSet<PerfilDeAcesso> PerfisDeAcesso { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<PerfilDeAcessoUsuario> PerfilDeAcessoUsuarios { get; set; }
        public DbSet<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; }
        public DbSet<PlanejamentoCusto> PlanejamentosCusto { get; set; }
        public DbSet<PlanejamentoCustoCargo> PlanejamentosCustoCargo { get; set; }
        public DbSet<RecursoWorkspace> RecursosWorkspace { get; set; }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Mapeia as entidades adicionadas antes que o estado mude para Unchanged
            var addedEntries = ChangeTracker.Entries<ITenantScoped>()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity)
                .ToList();

            // 2. Executa o salvamento principal, gerando os IDs reais no banco de dados
            var result = await base.SaveChangesAsync(cancellationToken);

            // 3. Processa o fluxo do tenant se houver registros e a trava estiver liberada
            if (addedEntries.Count != 0 && !IsProcessingTenantRecords)
            {
                IsProcessingTenantRecords = true;

                var workspaceCodigo = CurrentWorkspaceCodigo;
                if (!string.IsNullOrEmpty(workspaceCodigo))
                {
                    var workspaceId = await Workspaces
                        .Where(w => w.Codigo == workspaceCodigo)
                        .Select(w => w.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (workspaceId != 0)
                    {
                        foreach (var entity in addedEntries)
                        {
                            // Lê o código do módulo configurado no [TenantModule(...)] da classe
                            var moduloCodigo = ObterModuloCodigo(entity.GetType());

                            RecursosWorkspace.Add(new RecursoWorkspace
                            {
                                WorkspaceId = workspaceId,
                                WorkspaceCodigo = workspaceCodigo,
                                ModuloCodigo = moduloCodigo,
                                RecursoId = entity.Id, // Captura o ID real gerado pelo PostgreSQL
                                RecursoCodigo = entity.Codigo
                            });
                        }

                        // Salva os vínculos do tenant recém-criados
                        await base.SaveChangesAsync(cancellationToken);
                    }
                }

                IsProcessingTenantRecords = false;
            }

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);

            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                {
                    var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;

                    if (propertyType == typeof(DateTime))
                        property.SetColumnType("timestamp without time zone");
                }
            }

            // Mapeamento dinâmico dos filtros de tenant
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
            string moduloCodigo = ObterModuloCodigo(typeof(TEntity));

            // Configura o filtro onde qualquer consulta vai uar esse bloco
            modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
                string.IsNullOrEmpty(CurrentWorkspaceCodigo) ||
                this.RecursosWorkspace.Any(rw =>
                    rw.RecursoId == e.Id &&
                    rw.ModuloCodigo == moduloCodigo &&
                    rw.WorkspaceCodigo == CurrentWorkspaceCodigo));
        }

        // Obtém o código do atributo do módulo
        private string ObterModuloCodigo(Type entityType)
        {
            return entityType.ObterDescricaoDoModulo();
        }
    }
}