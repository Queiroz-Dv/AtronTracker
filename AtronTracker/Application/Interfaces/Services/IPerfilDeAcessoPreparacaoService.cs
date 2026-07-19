using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoPreparacaoService
    {
        Task<Resultado<PerfilDeAcesso>> PrepararAsync(PerfilDeAcessoDTO perfilDeAcessoDTO);
    }
}
