using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Mapping;
using Application.Records.Tarefa;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Shared.Application.Interfaces.Mapping;
using Shared.Extensions;
using System;

namespace Application.Mapping
{
    public sealed class TarefaMovimentacaoMapping
        : Mapper<TarefaMovimentacao, TarefaMovimentacaoDTO>,
          ITarefaMovimentacaoMapping
    {
        public TarefaMovimentacaoDTO MapearParaCriacao(
            Tarefa tarefa,
            Usuario responsavel)
        {
            var dto = CriarBase(tarefa, responsavel);
            dto.TipoMovimentacaoTarefa = TipoMovimentacaoTarefa.Criacao;
            dto.Detalhes = string.Format(
                TarefaResource.Historico_DetalheCriacao,
                tarefa.ObterEstado());
            return dto;
        }

        public TarefaMovimentacaoDTO MapearParaAtualizacao(
            AtualizacaoMovimentacaoRecord parametros)
        {
            var dto = CriarBase(parametros.TarefaAtual, parametros.Responsavel);
            dto.TipoMovimentacaoTarefa = TipoMovimentacaoTarefa.Atualizacao;
            dto.Detalhes = parametros.CriarDetalhesDaAtualizacao();
            return dto;
        }

        public TarefaMovimentacaoDTO MapearParaObtencao(
            Tarefa tarefa,
            Usuario responsavel)
        {
            var dto = CriarBase(tarefa, responsavel);
            dto.TipoMovimentacaoTarefa = TipoMovimentacaoTarefa.Obtencao;
            dto.Detalhes = string.Format(
                TarefaResource.Historico_DetalheObtencao,
                responsavel.ObterNome());
            return dto;
        }

        public TarefaMovimentacaoDTO MapearParaSolicitacao(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel)
        {
            var dto = CriarBase(solicitacao.TarefaId, responsavel);
            dto.TipoMovimentacaoTarefa = TipoMovimentacaoTarefa.SolicitacaoObtencao;
            dto.Detalhes = string.Format(
                TarefaResource.Historico_DetalheSolicitacao,
                solicitacao.Aprovador.ObterNome());
            return dto;
        }

        public TarefaMovimentacaoDTO MapearParaDecisao(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel,
            bool aprovar)
        {
            var dto = CriarBase(solicitacao.TarefaId, responsavel);
            dto.TipoMovimentacaoTarefa = aprovar ? TipoMovimentacaoTarefa.AprovacaoObtencao : TipoMovimentacaoTarefa.RecusaObtencao;

            dto.Detalhes = aprovar
                ? string.Format(
                    TarefaResource.Historico_DetalheAprovacao,
                    solicitacao.Solicitante.ObterNome(),
                    solicitacao.Tarefa.ObterEstado())
                : string.Format(
                    TarefaResource.Historico_DetalheRecusa,
                    solicitacao.Solicitante.ObterNome());
            return dto;
        }

        public override TarefaMovimentacaoDTO MapToDto(
            TarefaMovimentacao entity)
        {
            return new TarefaMovimentacaoDTO
            {
                Id = entity.Id,
                TarefaId = entity.TarefaId,
                Movimento = entity.Tipo.GetDescription(),
                TipoMovimentacaoTarefa = entity.Tipo,
                Detalhes = entity.Descricao,
                ResponsavelCodigo = entity.ResponsavelCodigo,
                ResponsavelNome = entity.ResponsavelNome,
                DataOcorrencia = DateTime.SpecifyKind(
                    entity.DataOcorrencia,
                    DateTimeKind.Utc)
            };
        }

        public override TarefaMovimentacao MapToEntity(
            TarefaMovimentacaoDTO dto)
        {
            return new TarefaMovimentacao
            {
                Id = dto.Id,
                TarefaId = dto.TarefaId,
                Tipo = dto.TipoMovimentacaoTarefa,
                Descricao = dto.Detalhes,
                ResponsavelCodigo = dto.ResponsavelCodigo,
                ResponsavelNome = dto.ResponsavelNome,
                DataOcorrencia = dto.DataOcorrencia
            };
        }

        private static TarefaMovimentacaoDTO CriarBase(
            Tarefa tarefa,
            Usuario responsavel)
        {
            return CriarBase(tarefa.Id, responsavel);
        }

        private static TarefaMovimentacaoDTO CriarBase(
            int tarefaId,
            Usuario responsavel)
        {
            return new TarefaMovimentacaoDTO
            {
                TarefaId = tarefaId,
                ResponsavelCodigo = responsavel.Codigo,
                ResponsavelNome = responsavel.ObterNome(),
                DataOcorrencia = DateTime.SpecifyKind(
                    DateTime.UtcNow,
                    DateTimeKind.Unspecified)
            };
        }
    }
}
