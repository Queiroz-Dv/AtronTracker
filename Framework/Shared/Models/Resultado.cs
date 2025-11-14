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

    /// <summary>
    /// Representa a classe base para o resultado de uma operação, 
    /// indicando sucesso ou falha e contendo mensagens de resposta.
    /// </summary>
    public class Resultado
    {
        /// <summary>
        /// Obtém um valor que indica se a operação foi bem-sucedida.
        /// </summary>
        public bool TeveSucesso { get; }

        /// <summary>
        /// Obtém um valor que indica se a operação falhou (o oposto de <see cref="TeveSucesso"/>).
        /// </summary>
        public bool TeveFalha => !TeveSucesso;

        /// <summary>
        /// Obtém a lista de mensagens (erros, avisos, sucesso) associadas à operação.
        /// </summary>
        public IList<Message> Response { get; }

        /// <summary>
        /// Construtor protegido para forçar o uso dos métodos de fábrica estáticos.
        /// </summary>
        /// <param name="teveSucesso">Indica se a operação foi bem-sucedida.</param>
        /// <param name="response">A lista de mensagens associadas.</param>
        protected Resultado(bool teveSucesso, IList<Message> response)
        {
            TeveSucesso = teveSucesso;
            Response = response ?? new List<Message>();
        }

        // --- MÉTODOS DE FÁBRICA NÃO-GENÉRICOS ---

        /// <summary>
        /// Cria um novo resultado de falha com uma lista de mensagens.
        /// </summary>
        /// <param name="response">A lista de mensagens de falha.</param>
        public static Resultado Falha(IList<Message> response)
        {
            return new Resultado(false, response);
        }

        /// <summary>
        /// Cria um novo resultado de falha com uma única string de erro.
        /// </summary>
        /// <param name="mensagemErro">A descrição do erro.</param>
        public static Resultado Falha(string mensagemErro)
        {
            var messages = new List<Message>
            {
                new Message { Descricao = mensagemErro, Nivel = Level.Error }
            };
            return new Resultado(false, messages);
        }

        /// <summary>
        /// Cria um novo resultado de falha extraindo as notificações de um <see cref="MessageModel"/>.
        /// </summary>
        /// <param name="messageModel">O modelo de mensagem contendo as notificações.</param>
        public static Resultado Falha(MessageModel messageModel)
        {
            return new Resultado(false, messageModel.Notificacoes);
        }

        /// <summary>
        /// Cria um novo resultado de sucesso (sem payload de dados).
        /// </summary>
        public static Resultado Sucesso()
        {
            return new Resultado(true, new List<Message>());
        }

        /// <summary>
        /// Cria um novo resultado de sucesso com uma lista de mensagens (ex: avisos ou info).
        /// </summary>
        /// <param name="response">A lista de mensagens de sucesso/aviso.</param>
        public static Resultado Sucesso(IList<Message> response)
        {
            return new Resultado(true, response);
        }

        // --- MÉTODOS DE FÁBRICA GENÉRICOS ---

        /// <summary>
        /// Cria um novo resultado de falha genérico com uma lista de mensagens.
        /// </summary>
        /// <typeparam name="T">O tipo do payload de dados (será 'default').</typeparam>
        /// <param name="response">A lista de mensagens de falha.</param>
        public static Resultado<T> Falha<T>(IList<Message> response)
        {
            return new Resultado<T>(false, default, response);
        }

        /// <summary>
        /// Cria um novo resultado de falha genérico com uma única string de erro.
        /// </summary>
        /// <typeparam name="T">O tipo do payload de dados (será 'default').</typeparam>
        /// <param name="mensagemErro">A descrição do erro.</param>
        public static Resultado<T> Falha<T>(string mensagemErro)
        {
            var messages = new List<Message>
            {
                new Message { Descricao = mensagemErro, Nivel = Level.Error }
            };
            return new Resultado<T>(false, default, messages);
        }

        /// <summary>
        /// Cria um novo resultado de falha genérico extraindo as notificações de um <see cref="MessageModel"/>.
        /// </summary>
        /// <typeparam name="T">O tipo do payload de dados (será 'default').</typeparam>
        /// <param name="messageModel">O modelo de mensagem contendo as notificações.</param>
        public static Resultado<T> Falha<T>(MessageModel messageModel)
        {
            return new Resultado<T>(false, default, messageModel.Notificacoes);
        }

        /// <summary>
        /// Cria um novo resultado de sucesso genérico com um payload de dados.
        /// </summary>
        /// <typeparam name="T">O tipo do payload de dados.</typeparam>
        /// <param name="data">O payload de dados.</param>
        public static Resultado<T> Sucesso<T>(T data)
        {
            return new Resultado<T>(true, data, new List<Message>());
        }

        /// <summary>
        /// Cria um novo resultado de sucesso genérico com um payload de dados e uma lista de mensagens.
        /// </summary>
        /// <typeparam name="T">O tipo do payload de dados.</typeparam>
        /// <param name="data">O payload de dados.</param>
        /// <param name="response">A lista de mensagens de sucesso/aviso.</param>
        public static Resultado<T> Sucesso<T>(T data, IList<Message> response)
        {
            return new Resultado<T>(true, data, response);
        }

        // --- MÉTODOS HELPERS ---

        /// <summary>
        /// Converte a lista <see cref="Response"/> em um formato serializável (anônimo) 
        /// para ser retornado em uma API.
        /// </summary>
        public IEnumerable<dynamic> ObterNotificacoes()
        {
            // Este método de extensão 'ConvertMessageToJson' deve existir em Shared.Extensions
            return Response.ConvertMessageToJson();
        }
    }
}