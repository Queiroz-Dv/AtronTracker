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

        public static new Resultado<T> Falha(IEnumerable<NotificationMessage> messages)
        {
            var resultado = new Resultado<T>(false);
             foreach (var message in messages)
            {
                resultado.Adicionar(message);
            }
            return resultado;
        }
    }

    public class Resultado : NotificationBag
    {
        public object? Dados { get; protected set; }

        public bool TeveSucesso => !Messages.Any(m => m.Nivel == ENotificationType.Error);

        [JsonIgnore]
        public bool TeveFalha => !TeveSucesso;

        protected Resultado(bool teveSucesso)
        {
            // O estado de sucesso/falha é inferido pelas notificações, 
            // mas o construtor pode ser usado para inicializar.
            // Se falha for forçada na construção, talvez devêssemos adicionar um erro genérico se não houver mensagens?
            // Por enquanto, seguimos a lógica de que "Sem erros = Sucesso".
        }

        public static Resultado Falha(string mensagemErro)
        {
            var resultado = new Resultado(false);
            resultado.AdicionarErro(mensagemErro);
            return resultado;
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

        // Adicionar(NotificationMessage) já existe em NotificationBag (via minha alteração anterior) ou eu adiciono aqui se não estiver lá
        // NotificationBag tem Adicionar(NotificationMessage) agora.
    }
}