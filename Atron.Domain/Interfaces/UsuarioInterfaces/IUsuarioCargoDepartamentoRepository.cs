using Atron.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioCargoDepartamentoRepository : IRepository<UsuarioCargoDepartamento>
    {
        Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo);

        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento);
    }
}