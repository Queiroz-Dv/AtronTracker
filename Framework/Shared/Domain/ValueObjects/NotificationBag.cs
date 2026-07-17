using Shared.Application.Resources;
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

        public void Adicionar(NotificationMessage message)
        {
            _messages.Add(message);
        }

        public void AdicionarErro(string description)
        {
            AddNotification(description, ENotificationType.Error);
        }

        public void AdicionarAviso(string description)
        {
            AddNotification(description, ENotificationType.Aviso);
        }

        public void AdicionarErroCampoObrigatorio(string campo)
        {
            var mensagemFormatada = string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, campo);
            AdicionarErro(mensagemFormatada);
        }

        public void AdicionarErroRegistroNulo()
        {
            AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
        }

        public void AdicionarMensagem(string description)
        {
            AddNotification(description, ENotificationType.Mensagem);
        }

        public void MensagemRegistroSalvo(string registro)
        {
            AddNotification(string.Format(ObterResource("Mensagem_EntidadeSalva"), registro), ENotificationType.Sucesso);
        }

        public void MensagemRegistroAtualizado(string registro)
        {
            AddNotification(string.Format(ObterResource("Mensagem_RegistroAtualizado"), registro), ENotificationType.Sucesso);
        }

        public void MensagemRegistroNaoEncontrado(string key = "")
        {
            AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoNaoEncontrado"), key));
        }

        public void MensagemRegistroRemovido(string registro = "")
        {
            if (registro.IsNullOrEmpty())
            {
                AdicionarMensagem(ObterResource("Mensagem_RemocaoSucessoSemRegistro"));
            }
            else
            {
                AdicionarMensagem(string.Format(ObterResource("Mensagem_RegistroRemovido"), registro));
            }

        }

        public void MensagemRegistroInvalido(string key = "")
        {
            AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoInvalido"), key));
        }

        public void MensagemRegistroNaoExiste(string key)
        {
            AdicionarErro(string.Format(ObterResource("Erro_RegistroComDescricaoExistente"), key));
        }

        private static string ObterResource(string chave)
            => NotificacoesPadronizadas.ResourceManager.GetString(chave) ?? throw new System.Resources.MissingManifestResourceException($"Resource não encontrado: {chave}");
    }
}
