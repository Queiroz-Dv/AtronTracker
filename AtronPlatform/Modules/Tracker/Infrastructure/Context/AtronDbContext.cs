using Domain.Entities;
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
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public AtronDbContext(DbContextOptions<AtronDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string WorkspaceCodigoAtual => _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimCode.CODIGO_WORKSPACE)?.Value ?? string.Empty;

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

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);

            ApplyTenantFilters(modelBuilder);

            if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                {
                    var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;

                    if (propertyType == typeof(DateTime))
                        property.SetColumnType("timestamp without time zone");
                }
            }
        }

       
    }
}
