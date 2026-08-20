using Application.DTO;
using Application.Resources;
using Domain.Enums;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Validador
{
    public class TarefaValidador : IValidador<TarefaDTO>
    {
        public IEnumerable<NotificationMessage> Validar(TarefaDTO tarefa)
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
                notificacoes.AdicionarErroCampoObrigatorio(TarefaResource.Campo_EstadoDaTarefa);
            }

            return notificacoes.Messages.ToList();
        }

        private static void ValidarDestinoInicial(TarefaDTO tarefa, NotificationBag notificacoes)
        {
            if (!Enum.IsDefined(typeof(DestinoInicialTarefa), tarefa.DestinoInicial))
            {
                notificacoes.AdicionarErroCampoObrigatorio(TarefaResource.Campo_DestinoInicial);
                return;
            }

            var destino = (DestinoInicialTarefa)tarefa.DestinoInicial;

            if (destino == DestinoInicialTarefa.Usuario)
            {
                ValidarCodigoObrigatorio(tarefa.UsuarioCodigo, TarefaResource.Campo_Usuario, notificacoes);
                return;
            }

            if (destino == DestinoInicialTarefa.DepartamentoCargo)
            {
                ValidarCodigoObrigatorio(tarefa.DepartamentoCodigo, TarefaResource.Campo_Departamento, notificacoes);
                ValidarCodigoOpcional(tarefa.CargoCodigo, TarefaResource.Campo_Cargo.ToLower(), notificacoes);
                return;
            }

            ValidarCodigoOpcional(tarefa.UsuarioCodigo, TarefaResource.Campo_Usuario.ToLower(), notificacoes);
            ValidarCodigoOpcional(tarefa.DepartamentoCodigo, TarefaResource.Campo_Departamento.ToLower(), notificacoes);
            ValidarCodigoOpcional(tarefa.CargoCodigo, TarefaResource.Campo_Cargo.ToLower(), notificacoes);
        }

        private static void ValidarConteudo(TarefaDTO tarefa, NotificationBag notificacoes)
        {
            if (tarefa.Titulo.IsNullOrEmpty())
            {
                notificacoes.AdicionarErroCampoObrigatorio(TarefaResource.Campo_Titulo);
            }

            if (tarefa.Titulo.Length > 50)
            {
                notificacoes.AdicionarErro(TarefaResource.Erro_TituloTamanhoMaximo);
            }

            if (!tarefa.Conteudo.IsNullOrEmpty() && tarefa.Conteudo.Length > 2500)
            {
                notificacoes.AdicionarErro(TarefaResource.Erro_ConteudoTamanhoMaximo);
            }

            if (tarefa.DataInicial > tarefa.DataFinal)
            {
                notificacoes.AdicionarErro(TarefaResource.Erro_PeriodoInvalido);
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
                notificacoes.AdicionarErro(string.Format(TarefaResource.Erro_CodigoTamanhoMinimo, campo));
            }

            if (codigo.Length > 10)
            {
                notificacoes.AdicionarErro(string.Format(TarefaResource.Erro_CodigoTamanhoMaximo, campo));
            }
        }
    }
}
