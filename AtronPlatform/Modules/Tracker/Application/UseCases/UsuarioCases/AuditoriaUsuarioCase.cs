using Domain.Entities;
using Shared.Application.Messaging;
using Shared.Domain.Events.Auditoria;

namespace Application.UseCases.UsuarioCases
{
    public sealed class AuditoriaUsuarioCase(IEventBus eventBus)
    {
        private readonly IEventBus _eventBus = eventBus;
        private const string UsuarioContexto = nameof(Usuario);

        public async Task ExecutarAsync(Usuario usuario)
        {
            await _eventBus.PublicarAsync(new AuditoriaRegistradaEvent(
                usuario.Codigo,
                UsuarioContexto,
                "Usuario $({usuario.Codigo}) criado em $({DateTime.Now:dd/MM/yyyy HH:mm})."
            ));
        }

        public async Task RegistrarAtualizacaoAsync(Usuario usuario)
        {
            await _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                usuario.Codigo,
                UsuarioContexto,
                "Usuário $({usuario.Codigo}) atualizado em $({DateTime.Now:dd/MM/yyyy HH:mm})."
            ));
        }
    }
}