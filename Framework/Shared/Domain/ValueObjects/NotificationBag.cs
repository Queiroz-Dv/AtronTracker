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

        public void AdicionarErros(IEnumerable<NotificationMessage> erros)
        {
            foreach (var erro in erros)
            {
                AdicionarErro(erro.Descricao);
            }
        }

        public void AdicionarAviso(string description)
        {
            AddNotification(description, ENotificationType.Aviso);
        }

        public void AdicionarAvisos(IEnumerable<NotificationMessage> avisos)
        {
            foreach (var aviso in avisos)
            {
                AdicionarAviso(aviso.Descricao);
            }
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
            AddNotification(string.Format(NotificacoesPadronizadas.Mensagem_EntidadeSalva, registro), ENotificationType.Sucesso);
        }

        public void MensagemRegistroAtualizado(string registro)
        {
            AddNotification(string.Format(NotificacoesPadronizadas.Mensagem_RegistroAtualizado, registro), ENotificationType.Sucesso);
        }

        public void MesagemFalhaNaGravacao(string registro)
        {
            AddNotification(string.Format(NotificacoesPadronizadas.Erro_FalhaNaGravacao, registro), ENotificationType.Sucesso);
        }

        public void MensagemRegistroNaoEncontrado(string key = "")
        {
            AdicionarErro(string.Format(NotificacoesPadronizadas.Erro_RegistroComDescricaoNaoEncontrado, key));
        }

        public void MensagemRegistroExistente(string key = "")
        {
            AdicionarErro(string.Format(NotificacoesPadronizadas.Erro_RegistroComDescricaoExistente, key));
        }

        public void MensagemRegistroRemovido(string registro = "")
        {
            if (registro.IsNullOrEmpty())
            {
                AdicionarMensagem(NotificacoesPadronizadas.Mensagem_RemocaoSucessoSemRegistro);
            }
            else
            {
                AdicionarMensagem(string.Format(NotificacoesPadronizadas.Mensagem_RegistroRemovido, registro));
            }

        }

        public void MensagemRegistroInvalido(string key = "")
        {
            AdicionarErro(string.Format(NotificacoesPadronizadas.Erro_RegistroComDescricaoInvalido, key));
        }

        public void MensagemRegistroNaoExiste(string key)
        {
            // Consertar depois 
            AdicionarErro(string.Format(NotificacoesPadronizadas.Erro_RegistroComDescricaoExistente, key));
        }
    }
}
