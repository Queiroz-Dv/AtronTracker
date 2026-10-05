using Application.DTO;
using Application.Resources;
using Domain.Enums;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Extensions;
using Shared.Extensions.RegraExtensions;
using System;

namespace Application.Validacoes
{
    public class TarefaValidador : Validador<TarefaDTO>
    {
        public TarefaValidador()
        {
            RegraPara(x => x.DestinoInicial)
                .DeveSer(d => Enum.IsDefined(typeof(DestinoInicialTarefa), d))
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, TarefaResource.Campo_DestinoInicial));

            RegraPara(x => x.UsuarioCodigo)
                .Quando(x => x.DestinoInicial == DestinoInicialTarefa.Usuario.GetDescription())
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, TarefaResource.Campo_Usuario));

            RegraPara(x => x.DepartamentoCodigo)
                .Quando(x => x.DestinoInicial == DestinoInicialTarefa.DepartamentoCargo.GetDescription())
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, TarefaResource.Campo_Departamento));

            RegraPara(x => x.UsuarioCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.UsuarioCodigo))
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMinimo, TarefaResource.Campo_Usuario.ToLower()));
            RegraPara(x => x.UsuarioCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.UsuarioCodigo))
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMaximo, TarefaResource.Campo_Usuario.ToLower()));

            RegraPara(x => x.DepartamentoCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.DepartamentoCodigo))
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMinimo, TarefaResource.Campo_Departamento.ToLower()));
            RegraPara(x => x.DepartamentoCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.DepartamentoCodigo))
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMaximo, TarefaResource.Campo_Departamento.ToLower()));

            RegraPara(x => x.CargoCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.CargoCodigo))
                .TamanhoMaiorOuIgualA(3)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMinimo, TarefaResource.Campo_Cargo.ToLower()));
            RegraPara(x => x.CargoCodigo)
                .Quando(x => !string.IsNullOrEmpty(x.CargoCodigo))
                .TamanhoMenorOuIgualA(10)
                .ComMensagem(string.Format(TarefaResource.Erro_CodigoTamanhoMaximo, TarefaResource.Campo_Cargo.ToLower()));

            RegraPara(x => x.Titulo)
                .NaoVazio()
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, TarefaResource.Campo_Titulo));

            RegraPara(x => x.Titulo)
                .TamanhoMenorOuIgualA(50)
                .ComMensagem(TarefaResource.Erro_TituloTamanhoMaximo);

            RegraPara(x => x.Conteudo)
                .Quando(x => !string.IsNullOrEmpty(x.Conteudo))
                .TamanhoMenorOuIgualA(2500)
                .ComMensagem(TarefaResource.Erro_ConteudoTamanhoMaximo);

            RegraPara(x => x)
                .DeveSer(t => t.DataInicial <= t.DataFinal)
                .ComMensagem(TarefaResource.Erro_PeriodoInvalido);

            RegraPara(x => x.EstadoDaTarefa)
                .DeveSer(e => e != null && e.Id > 0)
                .ComMensagem(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, TarefaResource.Campo_EstadoDaTarefa));
        }
    }
}
