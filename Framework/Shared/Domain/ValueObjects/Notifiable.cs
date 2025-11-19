using Shared.Application.Services;
using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects
{
    [Serializable]
    public abstract class Notifiable : MessageService
    {
        public override void AdicionarErro(string description)
        {
            AddNotification(description, ENotificationType.Error);
        }

        public override void AdicionarMensagem(string description)
        {
            AddNotification(description, ENotificationType.Mensagem);
        }

        public override void MensagemRegistroSalvo(string key)
        {
            AddNotification($"Registro {key} salvo com sucesso.", ENotificationType.Sucesso);
        }

        public override void MensagemRegistroAtualizado(string key)
        {
            AdicionarMensagem($"Registro {key} atualizado com sucesso.");
        }

        public override void MensagemRegistroNaoEncontrado(string key = "")
        {
            AdicionarErro($"Registro {key} não encontrado.");
        }

        public override void MensagemRegistroRemovido(string key = "")
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
            AddNotification(description, ENotificationType.Aviso);
        }
    }
}