using Shared.Extensions;

namespace Shared.Models
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
        internal Resultado(bool teveSucesso, T dado, IList<Message> response)
            : base(teveSucesso, response)
        {
            Dado = dado;
        }
    }
    
    public class Resultado
    {        
        public bool TeveSucesso { get; }       
        public bool TeveFalha => !TeveSucesso;       
        public IList<Message> Response { get; }
       
        protected Resultado(bool teveSucesso, IList<Message> response)
        {
            TeveSucesso = teveSucesso;
            Response = response ?? new List<Message>();
        }
        
        public static Resultado Falha(IList<Message> response)
        {
            return new Resultado(false, response);
        }
        
        public static Resultado Falha(string mensagemErro)
        {
            var messages = new List<Message>
            {
                new Message { Descricao = mensagemErro, Nivel = Level.Error }
            };
            return new Resultado(false, messages);
        }
        
        public static Resultado Falha(MessageModel messageModel)
        {
            return new Resultado(false, messageModel.Notificacoes);
        }
 
        public static Resultado Sucesso()
        {
            return new Resultado(true, new List<Message>());
        }

        public static Resultado Sucesso(IList<Message> response)
        {
            return new Resultado(true, response);
        }
        
        public static Resultado<T> Falha<T>(IList<Message> response)
        {
            return new Resultado<T>(false, default, response);
        }

        public static Resultado<T> Falha<T>(string mensagemErro)
        {
            var messages = new List<Message>
            {
                new Message { Descricao = mensagemErro, Nivel = Level.Error }
            };
            return new Resultado<T>(false, default, messages);
        }

        public static Resultado<T> Falha<T>(MessageModel messageModel)
        {
            return new Resultado<T>(false, default, messageModel.Notificacoes);
        }

        public static Resultado<T> Sucesso<T>(T data)
        {
            return new Resultado<T>(true, data, new List<Message>());
        }
        
        public static Resultado<T> Sucesso<T>(T data, IList<Message> response)
        {
            return new Resultado<T>(true, data, response);
        }

        public IEnumerable<dynamic> ObterNotificacoes()
        {
            return Response.ConvertMessageToJson();
        }
    }
}