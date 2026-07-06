using Application.DTO;
using Domain.Enums;
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

            ValidarDestinoInicial(tarefa, notificacoes);
            ValidarConteudo(tarefa, notificacoes);

            if (tarefa.EstadoDaTarefa is null || tarefa.EstadoDaTarefa.Id <= 0)
            {
                notificacoes.AdicionarErroCampoObrigatorio("Estado da tarefa");
            }

            return notificacoes.Messages.ToList();
        }

        private static void ValidarDestinoInicial(TarefaDTO tarefa, NotificationBag notificacoes)
        {
            if (!System.Enum.IsDefined(typeof(DestinoInicialTarefa), tarefa.DestinoInicial))
            {
                notificacoes.AdicionarErroCampoObrigatorio("Destino inicial da tarefa");
                return;
            }

            var destino = (DestinoInicialTarefa)tarefa.DestinoInicial;

            if (destino == DestinoInicialTarefa.Usuario)
            {
                ValidarCodigoObrigatorio(tarefa.UsuarioCodigo, "Usuario", notificacoes);
                return;
            }

            if (destino == DestinoInicialTarefa.DepartamentoCargo)
            {
                ValidarCodigoObrigatorio(tarefa.DepartamentoCodigo, "Departamento", notificacoes);
                ValidarCodigoOpcional(tarefa.CargoCodigo, "cargo", notificacoes);
                return;
            }

            ValidarCodigoOpcional(tarefa.UsuarioCodigo, "usuario", notificacoes);
            ValidarCodigoOpcional(tarefa.DepartamentoCodigo, "departamento", notificacoes);
            ValidarCodigoOpcional(tarefa.CargoCodigo, "cargo", notificacoes);
        }

        private static void ValidarConteudo(TarefaDTO tarefa, NotificationBag notificacoes)
        {
            if (tarefa.Titulo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErroCampoObrigatorio("Titulo");
            }
            else if (tarefa.Titulo.Length > 50)
            {
                notificacoes.AdicionarErro("O titulo da tarefa informada e invalido. Quantidade de caracteres maior que 50 digitos, tente novamente.");
            }

            if (!tarefa.Conteudo.IsNullOrEmpty() && tarefa.Conteudo.Length > 2500)
            {
                notificacoes.AdicionarErro("O conteudo da tarefa informada e invalido. Quantidade de caracteres maior que 2500 digitos, tente novamente.");
            }

            if (tarefa.DataInicial > tarefa.DataFinal)
            {
                notificacoes.AdicionarErro("Data inicial da tarefa e maior que a data final. Tente novamente.");
            }
        }

        private static void ValidarCodigoObrigatorio(string codigo, string campo, NotificationBag notificacoes)
        {
            if (codigo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErroCampoObrigatorio(campo);
                return;
            }

            ValidarCodigoOpcional(codigo, campo.ToLower(), notificacoes);
        }

        private static void ValidarCodigoOpcional(string codigo, string campo, NotificationBag notificacoes)
        {
            if (codigo.IsNullOrEmpty())
            {
                return;
            }

            if (codigo.Length < 3)
            {
                notificacoes.AdicionarErro($"O codigo de {campo} informado e invalido. Quantidade de caracteres menor que 3 digitos, tente novamente.");
            }

            if (codigo.Length > 10)
            {
                notificacoes.AdicionarErro($"O codigo de {campo} informado e invalido. Quantidade de caracteres maior que 10 digitos, tente novamente.");
            }
        }
    }
}
