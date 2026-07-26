using Application.DTO;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ISolicitacaoObtencaoTarefaMapeador
    {
        Task<SolicitacaoObtencaoTarefaDTO> MapearAsync(SolicitacaoObtencaoTarefa solicitacao);
    }
}
