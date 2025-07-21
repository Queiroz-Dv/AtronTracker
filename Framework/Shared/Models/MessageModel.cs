using Shared.Services;

namespace Shared.Models
{
    [Serializable]
    public abstract class MessageModel : MessageService
    {
        public override void AdicionarErro(string description)
        {
            AddNotification(description, Level.Error);
        }

        public override void AdicionarMensagem(string description)
        {
            AddNotification(description, Level.Message);
        }

        public override void MensagemRegistroSalvo(string key)
        {
            AdicionarMensagem($"Registro {key} salvo com sucesso.");
        }

        public override void MensagemRegistroAtualizado(string key)
        {
            AdicionarMensagem($"Registro {key} atualizado com sucesso.");
        }

        public override void MensagemRegistroNaoEncontrado(string key = "")
        {
            AdicionarErro($"Registro {key} não encontrado.");
        }

        public override void MensagemRegistroRemovido(string key)
        {
            AdicionarMensagem($"Registro {key} removido com sucesso");
        }

        public override void MensagemRegistroInvalido(string key = "")
        {
            AdicionarErro($"Registro {key} inválido");
        }

        public override void MensagemRegistroNaoExiste(string key)
        {
            AdicionarErro($"Registro {key} já existe.");
        }

        public override void AdicionarAviso(string description)
        {
            AddNotification(description, Level.Warning);
        }
    }
}