﻿using Atron.Domain.Componentes;
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

        public DbSet<Salario> Salarios { get; set; }       

        public DbSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public DbSet<PerfilDeAcesso> PerfisDeAcesso { get; set; }

        public DbSet<Modulo> Modulos { get; set; }

        public DbSet<PerfilDeAcessoUsuario> PerfilDeAcessoUsuarios { get; set; }

        public DbSet<PerfilDeAcessoModulo> PerfilDeAcessoModulos { get; set; }

        public DbSet<PropriedadesDeFluxo> PropriedadesDeFluxo { get; set; }
        public DbSet<PropriedadeDeFluxoModulo> PropriedadeDeFluxoModulo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtronDbContext).Assembly);
        }
    }
}