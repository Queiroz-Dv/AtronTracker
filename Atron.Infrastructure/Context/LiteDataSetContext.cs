using Atron.Domain.Entities;
using Shared.Models.ApplicationModels;

namespace Atron.Infrastructure.Context
{
    public abstract class LiteDataSetContext 
    {
        public IDataSet<Departamento> Departamentos { get; set; }

        public IDataSet<Cargo> Cargos { get; set; }

        public IDataSet<Usuario> Usuarios { get; set; }

        public IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public IDataSet<ApplicationUser> Users { get; set; }

        public IDataSet<ApplicationRole> Roles { get; set; }
    }
}