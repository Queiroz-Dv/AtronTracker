using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Services
{
    public abstract class MensagemRegistro
    {
        public abstract void MensagemRegistroNaoEncontrado(string key = "");

        public abstract void MensagemRegistroNaoExiste(string key = "");

        public abstract void MensagemRegistroSalvo(string key = "");

        public abstract void MensagemRegistroRemovido(string key = "");

        public abstract void MensagemRegistroAtualizado(string key = "");

        public abstract void MensagemRegistroInvalido(string key = "");
    }

    public abstract class MessageService : MensagemRegistro, IMessageService
    {
        public List<NotificationMessage> Notificacoes { get; }

        public abstract void AdicionarMensagem(string message);

        public abstract void AdicionarErro(string message);

        public abstract void AdicionarAviso(string message);

        /// <summary>
        /// Método de automação para inclusão de notificações
        /// </summary>
        /// <param name="descricao">Mensagem de notificação</param>
        /// <param name="nivel">Tipo de notificação</param>
        public void AddNotification(string descricao, string nivel)
        {
            Notificacoes.Add(new NotificationMessage() { Descricao = descricao, Nivel = nivel });
        }
    }
}