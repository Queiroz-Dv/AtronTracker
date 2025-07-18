using Atron.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPerfilDeAcessoUsuarioRepository : IRepository<PerfilDeAcessoUsuario>
    {
        Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso);
        Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo);

        Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento);
    }
}