using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioCargoDepartamentoRepository 
    {
        Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo);

        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento);

        public Task<bool> AtualizarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento);

        Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo);

        Task<bool> RemoverRelacionamentoPorDepartamentoRepository(UsuarioCargoDepartamento item);

        Task<bool> RemoverRelacionamentoPorId(int id);
    }
}