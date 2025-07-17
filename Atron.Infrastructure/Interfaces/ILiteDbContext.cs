using Atron.Domain.Entities;
using Atron.Infrastructure.Context;
using Shared.Models.ApplicationModels;

namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteDbContext 
    {
        IDataSet<Departamento> Departamentos { get; }

        IDataSet<Cargo> Cargos { get; }

        IDataSet<Usuario> Usuarios { get; }

        IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; }

        IDataSet<ApplicationUser> Users { get;  }

        IDataSet<ApplicationRole> Roles { get;  }
    }
}