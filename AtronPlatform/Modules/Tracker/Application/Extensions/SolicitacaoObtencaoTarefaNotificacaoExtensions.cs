using Application.Resources;
using Application.Records.Tarefa;
using Application.Statics;
using AtronNotificacoes.Contracts.DTO;
using AtronNotificacoes.Domain.Enums;
using Domain.Entities;
using Domain.Extensions;
using System;

namespace Application.Extensions
{
    public static class SolicitacaoObtencaoTarefaNotificacaoExtensions
    {
        public static PublicarNotificacaoInternaDto CriarNotificacaoDeRecebimento(
            this SolicitacaoObtencaoTarefa solicitacao)
        {
            const string tipoEvento = TarefaNotificacaoEventos.SolicitacaoObtencaoRecebida;
            var usuarioCodigo = solicitacao.AprovadorCodigo;
            var contexto = ContextoNotificacaoTarefaRecord.Criar(solicitacao);
            var solicitante = solicitacao.Solicitante?.ObterNome() ?? solicitacao.SolicitanteCodigo;

            return new PublicarNotificacaoInternaDto
            {
                DestinatarioCodigo = usuarioCodigo,
                ModuloOrigem = ENotificacaoModulos.Tracker,
                TipoEvento = tipoEvento,
                Titulo = TarefaResource.Titulo_SolicitacaoRecebida,
                Mensagem = string.Format(
                    TarefaResource.Mensagem_SolicitacaoRecebida,
                    solicitante,
                    contexto.TarefaIdTexto),
                UrlDestino = contexto.UrlSolicitacoes,
                ReferenciaExterna = contexto.ReferenciaExterna,
                DataCriacao = DateTimeOffset.UtcNow,
                ChaveIdempotencia = contexto.CriarChaveIdempotencia(
                    tipoEvento,
                    usuarioCodigo),
                CorrelacaoId = contexto.CriarCorrelacaoId(tipoEvento)
            };
        }

        public static PublicarNotificacaoInternaDto CriarNotificacaoDeDecisao(
            this SolicitacaoObtencaoTarefa solicitacao,
            bool aprovada)
        {
            var usuarioCodigo = solicitacao.SolicitanteCodigo;
            var contexto = ContextoNotificacaoTarefaRecord.Criar(solicitacao);

            var detalhesDecisao = CriarDetalhesDecisao(aprovada, contexto.TarefaIdTexto);

            var tipoEvento = detalhesDecisao.TipoEvento;
            var titulo = detalhesDecisao.Titulo;
            var mensagem = detalhesDecisao.Mensagem;

            return new PublicarNotificacaoInternaDto
            {
                DestinatarioCodigo = usuarioCodigo,
                ModuloOrigem = ENotificacaoModulos.Tracker,
                TipoEvento = tipoEvento,
                Titulo = titulo,
                Mensagem = mensagem,
                UrlDestino = contexto.UrlEdicao,
                ReferenciaExterna = contexto.ReferenciaExterna,
                DataCriacao = DateTimeOffset.UtcNow,
                ChaveIdempotencia = contexto.CriarChaveIdempotencia(
                    tipoEvento,
                    usuarioCodigo),
                CorrelacaoId = contexto.CriarCorrelacaoId(tipoEvento)
            };
        }

        private static (string TipoEvento, string Titulo, string Mensagem) CriarDetalhesDecisao(bool aprovada, string tarefaId)
        {
            var tipoEvento = aprovada
               ? TarefaNotificacaoEventos.SolicitacaoObtencaoAprovada
               : TarefaNotificacaoEventos.SolicitacaoObtencaoRecusada;

            var titulo = aprovada
               ? TarefaResource.Titulo_SolicitacaoAprovada
               : TarefaResource.Titulo_SolicitacaoRecusada;

            var mensagem = string.Format(
                aprovada
                    ? TarefaResource.Mensagem_NotificacaoSolicitacaoAprovada
                    : TarefaResource.Mensagem_NotificacaoSolicitacaoRecusada,
                tarefaId);

            return (tipoEvento, titulo, mensagem);
        }
    }
}
