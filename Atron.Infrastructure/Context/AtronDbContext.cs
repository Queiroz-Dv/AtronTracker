using Atron.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Models.ApplicationModels;

namespace Atron.Infrastructure.Context
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

        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Tarefa> Tarefas { get; set; }

        public DbSet<TarefaEstado> TarefaEstados { get; set; }

        public DbSet<Salario> Salarios { get; set; }       

        public DbSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);
        }
    }
}