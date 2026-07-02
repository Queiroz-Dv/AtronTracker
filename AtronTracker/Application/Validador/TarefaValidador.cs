using Application.DTO;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validador
{
    public class TarefaValidador : IValidador<TarefaDTO>
    {
        public IList<NotificationMessage> Validar(TarefaDTO tarefa)
        {
            var notificacoes = new NotificationBag();

            if (tarefa is null)
            {
                notificacoes.AdicionarErroRegistroNulo();
                return notificacoes.Messages.ToList();
            }

            if (tarefa.UsuarioCodigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErroCampoObrigatorio("Usuário");
            }
            else
            {
                if (tarefa.UsuarioCodigo.Length < 3)
                    notificacoes.AdicionarErro("O código de usuário informado é inválido. Quantidade de caracteres menor que 3 dígitos, tente novamente.");

                if (tarefa.UsuarioCodigo.Length > 10)
                    notificacoes.AdicionarErro("O código de usuário informado é inválido. Quantidade de caracteres maior que 10 dígitos, tente novamente.");
            }

            if (tarefa.Titulo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErroCampoObrigatorio("Título");
            }
            else if (tarefa.Titulo.Length > 50)
            {
                notificacoes.AdicionarErro("O título da tarefa informada é inválido. Quantidade de caracteres maior que 50 dígitos, tente novamente.");
            }

            if (!tarefa.Conteudo.IsNullOrEmpty() && tarefa.Conteudo.Length > 2500)
            {
                notificacoes.AdicionarErro("O conteúdo da tarefa informada é inválido. Quantidade de caracteres maior que 2500 dígitos, tente novamente.");
            }

            if (tarefa.DataInicial > tarefa.DataFinal)
            {
                notificacoes.AdicionarErro("Data inicial da tarefa é maior que a data final. Tente novamente.");
            }

            if (tarefa.EstadoDaTarefa is null || tarefa.EstadoDaTarefa.Id <= 0)
            {
                notificacoes.AdicionarErroCampoObrigatorio("Estado da tarefa");
            }

            return notificacoes.Messages.ToList();
        }
    }
}
