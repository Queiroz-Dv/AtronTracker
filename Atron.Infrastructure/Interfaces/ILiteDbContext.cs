using Atron.Domain.Entities;
using Atron.Infrastructure.Context;
using LiteDB;

namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteDbContext
    {
        LiteDatabase _db { get; }

        IDataSet<Departamento> Departamentos { get; }

        IDataSet<Cargo> Cargos { get; }

        IDataSet<Usuario> Usuarios { get; }

        IDataSet<UsuarioIdentity> UsuarioIdentity { get; }

        IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; }

        IDataSet<Modulo> Modulos { get; }

        IDataSet<PerfilDeAcesso> PerfisDeAcesso { get; }
    }
}