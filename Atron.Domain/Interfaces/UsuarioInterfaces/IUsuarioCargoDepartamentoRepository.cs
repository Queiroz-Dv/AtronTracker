using Atron.Tracker.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioCargoDepartamentoRepository : IRepository<UsuarioCargoDepartamento>
    {
        public Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo);

        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento);

        public Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo);
    }
}