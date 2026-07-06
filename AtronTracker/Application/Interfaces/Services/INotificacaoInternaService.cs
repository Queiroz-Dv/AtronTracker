using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface INotificacaoInternaService
    {
        Task<Resultado<List<NotificacaoInternaDTO>>> ObterMinhasAsync();

        Task<Resultado<NotificacaoInternaDTO>> MarcarComoLidaAsync(int id);

        Task<Resultado<NotificacaoInternaDTO>> CriarAsync(NotificacaoInterna notificacao);
    }
}
