using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoUsuarioRelacionamentoService
    {
        Task<Resultado> RelacionarAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario);
    }
}
