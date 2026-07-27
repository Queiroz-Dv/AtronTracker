using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IRecuperacaoSenhaService
    {
        Task<Resultado> SolicitarAsync(SolicitarRecuperacaoSenhaRequest request);

        Task<Resultado> TrocarAsync(RedefinirSenhaRequest request);
    }
}
