using Shared.Domain.Enums;
using Shared.Extensions;

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
        public T Dado { get; }

        /// <summary>
        /// Construtor interno para forçar o uso dos métodos de fábrica estáticos.
        /// </summary>
        /// <param name="teveSucesso">Indica se a operação foi bem-sucedida.</param>
        /// <param name="dado">O payload de dados.</param>
        /// <param name="response">A lista de mensagens associadas.</param>
        internal Resultado(bool teveSucesso, T dado, IList<NotificationMessage> response)
            : base(teveSucesso, response)
        {
            Dado = dado;
        }
    }
    
    public class Resultado
    {        
        public bool TeveSucesso { get; }       
        public bool TeveFalha => !TeveSucesso;       
        public IList<NotificationMessage> Response { get; }
       
        protected Resultado(bool teveSucesso, IList<NotificationMessage> response)
        {
            TeveSucesso = teveSucesso;
            Response = response ?? new List<NotificationMessage>();
        }
        
        public static Resultado Falha(IList<NotificationMessage> response)
        {
            return new Resultado(false, response);
        }
        
        public static Resultado Falha(string mensagemErro)
        {
            var messages = new List<NotificationMessage>
            {
                new NotificationMessage { Descricao = mensagemErro, Nivel = ENotificationType.Error }
            };
            return new Resultado(false, messages);
        }
        
        public static Resultado Falha(Notifiable messageModel)
        {
            return new Resultado(false, messageModel.Notificacoes);
        }
 
        public static Resultado Sucesso()
        {
            return new Resultado(true, new List<NotificationMessage>());
        }

        public static Resultado Sucesso(IList<NotificationMessage> response)
        {
            return new Resultado(true, response);
        }
        
        public static Resultado<T> Falha<T>(IList<NotificationMessage> response)
        {
            return new Resultado<T>(false, default, response);
        }

        public static Resultado<T> Falha<T>(string mensagemErro)
        {
            var messages = new List<NotificationMessage>
            {
                new NotificationMessage { Descricao = mensagemErro, Nivel = ENotificationType.Error }
            };
            return new Resultado<T>(false, default, messages);
        }

        public static Resultado<T> Falha<T>(Notifiable messageModel)
        {
            return new Resultado<T>(false, default, messageModel.Notificacoes);
        }

        public static Resultado<T> Sucesso<T>(T data)
        {
            return new Resultado<T>(true, data, new List<NotificationMessage>());
        }
        
        public static Resultado<T> Sucesso<T>(T data, IList<NotificationMessage> response)
        {
            return new Resultado<T>(true, data, response);
        }

        public IEnumerable<dynamic> ObterNotificacoes()
        {
            return Response.ConvertMessageToJson();
        }
    }
}