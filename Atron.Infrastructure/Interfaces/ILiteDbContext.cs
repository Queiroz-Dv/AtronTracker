using Atron.Domain.Entities;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteDbContext
    {
        IDataSet<Departamento> Departamentos { get; }

        IDataSet<Cargo> Cargos { get; }

        IDataSet<Usuario> Usuarios { get; }

        IDataSet<UsuarioIdentity> UsuarioIdentity { get; }

        IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; }

        IDataSet<Modulo> Modulos { get; }

        IDataSet<PerfilDeAcesso> PerfisDeAcesso { get; }

        IDataSet<PerfilDeAcessoModulo> PerfisDeAcessoModulo { get; }

        IDataSet<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario { get; }

        IDataSet<Tarefa> Tarefas { get; }

        IDataSet<TarefaEstado> TarefasEstados { get; }

        IDataSet<Salario> Salarios { get; }

        IDataSet<Mes> Meses { get; }
    }
}