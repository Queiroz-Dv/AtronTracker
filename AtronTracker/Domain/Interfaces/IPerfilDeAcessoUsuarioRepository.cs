using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPerfilDeAcessoUsuarioRepository
    {
        Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso);

        Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo);

        Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento);
    }
}