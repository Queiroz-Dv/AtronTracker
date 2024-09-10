using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atron.Infrastructure.Context
{
    public class AtronDbContext : DbContext
    {
        public AtronDbContext(DbContextOptions<AtronDbContext> options) : base(options) { }

        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Mes> Meses { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Tarefa> Tarefas { get; set; }

        public DbSet<TarefaEstado> TarefaEstados { get; set; }

        public DbSet<Salario> Salarios { get; set; }

        public DbSet<Permissao> Permissoes { get; set; }

        public DbSet<PermissaoEstado> PermissoesEstados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);
        }
    }
}