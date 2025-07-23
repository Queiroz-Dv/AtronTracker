using Atron.Domain.Entities;
using Atron.Infrastructure.Interfaces;

namespace Atron.Infrastructure.Context
{
    public abstract class LiteDataSetContext : ILiteDbContext
    {
        public IDataSet<Departamento> Departamentos { get; set; }

        public IDataSet<Cargo> Cargos { get; set; }

        public IDataSet<Usuario> Usuarios { get; set; }

        public IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public IDataSet<UsuarioIdentity> UsuarioIdentity { get; set; }

        public IDataSet<Modulo> Modulos { get; set; }

        public IDataSet<PerfilDeAcesso> PerfisDeAcesso { get; set; }

        public IDataSet<PerfilDeAcessoModulo> PerfisDeAcessoModulo { get; set; }

        public IDataSet<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario {  get; set; }
    }
}