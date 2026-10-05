using Application.Events;
using Application.Interfaces.Services;
using Application.UseCases.TarefaCases;
using Microsoft.Extensions.Logging;
using Shared.Application.Messaging;
using Shared.Extensions;

namespace Application.EventHandlers
{
    public class TarefaNotificacaoEventHandler(
        TarefaNotificacaoInternaCase notificacaoInternaCase,
        ITarefaNotificacaoService tarefaNotificacaoService,
        ILogger<TarefaNotificacaoEventHandler> logger) : IEventHandler<TarefaNotificacaoEvent>
    {
        private readonly TarefaNotificacaoInternaCase _notificacaoInternaCase = notificacaoInternaCase;
        private readonly ITarefaNotificacaoService _tarefaNotificacaoService = tarefaNotificacaoService;
        private readonly ILogger<TarefaNotificacaoEventHandler> _logger = logger;

        public async Task ManipularAsync(TarefaNotificacaoEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.NotificacaoInterna.IsNotNull())
            {
                await _notificacaoInternaCase.ExecutarAsync(@event.NotificacaoInterna);
                _logger.LogInformation("Notificação interna publicada de forma assíncrona para a tarefa.");
            }

            if (@event.TarefaEmail.IsNotNull() && @event.UsuarioEmail.IsNotNull())
            {
                var envioEmail = await _tarefaNotificacaoService.NotificarAtribuicaoAsync(@event.TarefaEmail, @event.UsuarioEmail);
                if (envioEmail.TeveFalha)
                {
                    _logger.LogWarning("Falha ao enviar email assíncrono de atribuição de tarefa: {Mensagem}", string.Join(", ", envioEmail.Messages));
                }
                else
                {
                    _logger.LogInformation("Email de atribuição de tarefa enviado de forma assíncrona com sucesso.");
                }
            }
        }
    }
}
