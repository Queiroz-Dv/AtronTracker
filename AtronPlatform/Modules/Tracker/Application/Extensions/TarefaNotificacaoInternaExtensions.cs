using Application.DTO;
using Application.Records.Tarefa;
using Application.Resources;
using Application.Statics;
using AtronNotificacoes.Contracts.DTO;
using AtronNotificacoes.Domain.Enums;
using Domain.Entities;
using System;

namespace Application.Extensions
{
    public static class TarefaNotificacaoInternaExtensions
    {
        public static PublicarNotificacaoInternaDto? CriarNotificacaoDeAtribuicao(
            this TarefaDTO tarefa)
        {
            if (tarefa.Usuario is null)
                return null;

            const string tipoEvento = TarefaNotificacaoEventos.TarefaAtribuida;
            var usuarioCodigo = tarefa.Usuario.Codigo;
            var contexto = ContextoNotificacaoTarefaRecord.Criar(tarefa);

            return new PublicarNotificacaoInternaDto
            {
                DestinatarioCodigo = usuarioCodigo,
                ModuloOrigem = ENotificacaoModulos.Tracker,
                TipoEvento = tipoEvento,
                Titulo = TarefaResource.Titulo_TarefaAtribuida,
                Mensagem = string.Format(
                    TarefaResource.Mensagem_TarefaAtribuidaUsuario,
                    contexto.TarefaIdTexto),
                UrlDestino = contexto.UrlEdicao,
                ReferenciaExterna = contexto.ReferenciaExterna,
                DataCriacao = DateTimeOffset.UtcNow,
                ChaveIdempotencia = contexto.CriarChaveIdempotencia(
                    tipoEvento,
                    usuarioCodigo),
                CorrelacaoId = contexto.CriarCorrelacaoId(tipoEvento)
            };
        }

        public static PublicarNotificacaoInternaDto CriarNotificacaoDeObtencao(
            this Tarefa tarefa,
            Usuario usuario)
        {
            const string tipoEvento = TarefaNotificacaoEventos.TarefaObtida;
            var contexto = ContextoNotificacaoTarefaRecord.Criar(tarefa);

            return new PublicarNotificacaoInternaDto
            {
                DestinatarioCodigo = usuario.Codigo,
                ModuloOrigem = ENotificacaoModulos.Tracker,
                TipoEvento = tipoEvento,
                Titulo = TarefaResource.Titulo_TarefaObtida,
                Mensagem = string.Format(
                    TarefaResource.Mensagem_TarefaObtida,
                    contexto.TarefaIdTexto),
                UrlDestino = contexto.UrlEdicao,
                ReferenciaExterna = contexto.ReferenciaExterna,
                DataCriacao = DateTimeOffset.UtcNow,
                ChaveIdempotencia = contexto.CriarChaveIdempotencia(
                    tipoEvento,
                    usuario.Codigo),
                CorrelacaoId = contexto.CriarCorrelacaoId(tipoEvento)
            };
        }
    }
}
