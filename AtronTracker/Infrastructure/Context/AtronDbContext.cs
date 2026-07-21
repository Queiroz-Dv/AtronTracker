using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.Identity;

namespace AtronTracker.Infrastructure.Context
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
        public AtronDbContext(DbContextOptions<AtronDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> AppUsers { get; set; }

        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<ConfirmacaoEmail> ConfirmacoesEmail { get; set; }

        public DbSet<Tarefa> Tarefas { get; set; }

        public DbSet<SolicitacaoObtencaoTarefa> SolicitacoesObtencaoTarefa { get; set; }

        public DbSet<TarefaEstado> TarefaEstados { get; set; }

        public DbSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public DbSet<PerfilDeAcesso> PerfisDeAcesso { get; set; }

        public DbSet<Modulo> Modulos { get; set; }

        public DbSet<PerfilDeAcessoUsuario> PerfilDeAcessoUsuarios { get; set; }

        public DbSet<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; }

        public DbSet<PlanejamentoCusto> PlanejamentosCusto { get; set; }

        public DbSet<PlanejamentoCustoCargo> PlanejamentosCustoCargo { get; set; }
         
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
        }
    }
}
