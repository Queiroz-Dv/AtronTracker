using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoUsuarioSincronizacaoService
    {
        Task<Resultado> SincronizarAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario);
    }
}
