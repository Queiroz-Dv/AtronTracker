using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects
{
    [Serializable]
    public abstract class Notifiable : MessageService
    {
        public override void AdicionarErro(string description) => AddNotification(description, ENotificationType.Error);

        public override void AdicionarMensagem(string description) => AddNotification(description, ENotificationType.Mensagem);

        public override void MensagemRegistroSalvo(string key) => AddNotification(string.Format(ObterResource("Mensagem_RegistroSalvo"), key), ENotificationType.Sucesso);

        public override void MensagemRegistroAtualizado(string key) => AdicionarMensagem(string.Format(ObterResource("Mensagem_RegistroAtualizado"), key));

        public override void MensagemRegistroNaoEncontrado(string key = "") => AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoNaoEncontrado"), key));

        public override void MensagemRegistroRemovido(string key = "") => AdicionarMensagem(string.Format(ObterResource("Mensagem_RegistroRemovido"), key));

        public override void MensagemRegistroInvalido(string key = "") => AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoInvalido"), key));

        public override void MensagemRegistroNaoExiste(string key) => AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoExistente"), key));

        public override void AdicionarAviso(string description) => AddNotification(description, ENotificationType.Aviso);

        private static string ObterResource(string chave)
            => NotificacoesPadronizadas.ResourceManager.GetString(chave) ?? throw new System.Resources.MissingManifestResourceException($"Resource não encontrado: {chave}");
    }
}
