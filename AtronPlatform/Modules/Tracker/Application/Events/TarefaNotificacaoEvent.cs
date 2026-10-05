using Application.DTO;
using AtronNotificacoes.Contracts.DTO;
using Shared.Domain.Events;

namespace Application.Events
{
    public class TarefaNotificacaoEvent : IEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime DataOcorrencia { get; } = DateTime.UtcNow;

        public PublicarNotificacaoInternaDto? NotificacaoInterna { get; }
        public TarefaDTO? TarefaEmail { get; }
        public UsuarioDTO? UsuarioEmail { get; }

        public TarefaNotificacaoEvent(
            PublicarNotificacaoInternaDto? notificacaoInterna,
            TarefaDTO? tarefaEmail = null,
            UsuarioDTO? usuarioEmail = null)
        {
            NotificacaoInterna = notificacaoInterna;
            TarefaEmail = tarefaEmail;
            UsuarioEmail = usuarioEmail;
        }
    }
}
