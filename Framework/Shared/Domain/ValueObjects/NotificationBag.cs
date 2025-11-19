using Shared.Domain.Enums;
using Shared.Extensions;

namespace Shared.Domain.ValueObjects
{    
    public class NotificationBag
    {
        private readonly List<NotificationMessage> _messages;

        public NotificationBag() => _messages = [];

        public IReadOnlyCollection<NotificationMessage> Messages => _messages.AsReadOnly();

        public void AddNotification(string description, string level)
        {
            _messages.Add(new NotificationMessage { Descricao = description, Nivel = level });
        }

        public void AdicionarErro(string description)
        {
            AddNotification(description, ENotificationType.Error);
        }

        public void AdicionarAviso(string description)
        {
            AddNotification(description, ENotificationType.Aviso);
        }

        public void AdicionarMensagem(string description)
        {
            AddNotification(description, ENotificationType.Mensagem);
        }

        public void MensagemRegistroSalvo(string registro)
        {
            AddNotification($"{registro} salvo com sucesso.", ENotificationType.Sucesso);
        }

        public void MensagemRegistroAtualizado(string registro)
        {
            AddNotification($"{registro} atualizado com sucesso.", ENotificationType.Sucesso);
        }

        public void MensagemRegistroNaoEncontrado(string key = "")
        {
            AdicionarErro($"Registro {key} não encontrado.");
        }

        public void MensagemRegistroRemovido(string registro = "")
        {
            if (registro.IsNullOrEmpty())
            {
                AdicionarMensagem($"Removido com sucesso");
            }
            else
            {
                AdicionarMensagem($"{registro} removido com sucesso");
            }

        }

        public void MensagemRegistroInvalido(string key = "")
        {
            AdicionarErro($"Registro {key} inválido");
        }

        public void MensagemRegistroNaoExiste(string key)
        {
            AdicionarErro($"Registro {key} já existe.");
        }
    }
}
