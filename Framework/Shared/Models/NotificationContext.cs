using Shared.Extensions;

namespace Shared.Models
{    
    public class NotificationContext
    {
        private readonly List<Message> _messages;

        public NotificationContext() => _messages = [];

        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

        public void AddNotification(string description, string level)
        {
            _messages.Add(new Message { Descricao = description, Nivel = level });
        }

        public void AdicionarErro(string description)
        {
            AddNotification(description, Level.Error);
        }

        public void AdicionarAviso(string description)
        {
            AddNotification(description, Level.Aviso);
        }

        public void AdicionarMensagem(string description)
        {
            AddNotification(description, Level.Mensagem);
        }

        public void MensagemRegistroSalvo(string registro)
        {
            AddNotification($"{registro} salvo com sucesso.", Level.Sucesso);
        }

        public void MensagemRegistroAtualizado(string registro)
        {
            AddNotification($"{registro} atualizado com sucesso.", Level.Sucesso);
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
