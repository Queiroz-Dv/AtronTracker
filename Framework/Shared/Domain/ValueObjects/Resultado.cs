using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Extensions;
using System.Text.Json.Serialization;

namespace Shared.Domain.ValueObjects
{
    /// <summary>
    /// Representa o resultado de uma operação com um payload de dados específico.
    /// Herda de <see cref="Resultado"/> não-genérico.
    /// </summary>
    /// <typeparam name="T">O tipo do payload de dados.</typeparam>
    public class Resultado<T> : Resultado
    {
        /// <summary>
        /// Obtém o payload de dados associado a um resultado de sucesso.
        /// </summary>
        public new T? Dados
        {
            get => (T?)base.Dados;
            private set => base.Dados = value;
        }

        /// <summary>
        /// Construtor interno para forçar o uso dos métodos de fábrica estáticos.
        /// </summary>
        internal Resultado(bool teveSucesso, T? dado = default)
            : base(teveSucesso)
        {
            Dados = dado;
        }

        public static new Resultado<T> Sucesso(T data)
        {
            return new Resultado<T>(true, data);
        }

        public static new Resultado<T> Falha(T data, IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado<T>(false, data);

            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }

            return resultado;
        }

        public static new Resultado<T> Sucesso(T data, IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado<T>(true, data);
            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }

        public static new Resultado<T> Falha(string mensagemErro)
        {
            var resultado = new Resultado<T>(false);
            resultado.AdicionarErro(mensagemErro);
            return resultado;
        }

        public static new Resultado<T> Falha(NotificationMessage message)
        {
            var resultado = new Resultado<T>(false);
            resultado.Adicionar(message);
            return resultado;
        }

        public static new Resultado<T> Falhas(IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado<T>(false);
            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }

        public new Resultado<T> AdicionarMensagem(string mensagem)
        {
            base.AdicionarMensagem(mensagem);
            return this;
        }
    }

    public class Resultado
    {
        private readonly List<NotificationMessage> _messages;

        public IReadOnlyCollection<NotificationMessage> Messages => _messages.AsReadOnly();

        public object? Dados { get; protected set; }

        public bool TeveSucesso => !_messages.Any(m => m.Nivel == ENotificationType.Error);

        [JsonIgnore]
        public bool TeveFalha => !TeveSucesso;

        public Resultado()
        {
            _messages = [];
        }

        protected Resultado(bool teveSucesso)
        {
            _messages = [];
            // O estado de sucesso/falha é inferido pelas notificações, 
            // mas o construtor pode ser usado para inicializar.
        }

        // --- Resultado Flattened Methods ---
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

        public void AdicionarMensagemBase(string description)
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
                AdicionarMensagemBase(NotificacoesPadronizadas.Mensagem_RemocaoSucessoSemRegistro);
            }
            else
            {
                AdicionarMensagemBase(string.Format(NotificacoesPadronizadas.Mensagem_RegistroRemovido, registro));
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
        // ----------------------------------------

        public static Resultado Falha(string mensagemErro)
        {
            var resultado = new Resultado(false);
            resultado.AdicionarErro(mensagemErro);
            return resultado;
        }

        public static Resultado Falha()
        {
            return new Resultado(false);                        
        }

        public static Resultado Falha(NotificationMessage message)
        {
            var resultado = new Resultado(false);
            resultado.Adicionar(message);
            return resultado;
        }
        
        public static Resultado Falha(IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado(false);
            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }

        public static Resultado Sucesso()
        {
            return new Resultado(true);
        }

        public static Resultado Sucesso(object dados)
        {
            return new Resultado(true) { Dados = dados };
        }

        public static Resultado Sucesso(string mensagem)
        {
            var resultado = new Resultado(true);
            resultado.AdicionarMensagem(mensagem);
            return resultado;
        }

        public static Resultado Sucesso(object dados, IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado(true) { Dados = dados };
            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }

        public static Resultado Sucesso(IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado(true);
            foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }

        public void Adicionar(string mensagem, string tipo)
        {
            AddNotification(mensagem, tipo);
        }

        public Resultado AdicionarMensagem(string mensagem)
        {
            AddNotification(mensagem, ENotificationType.Sucesso);
            return this;
        }       
    }
}
