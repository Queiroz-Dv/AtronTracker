using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mapping
{

    public sealed class SolicitacaoObtencaoTarefaMapping(
        IToDtoMapper<Tarefa, TarefaDTO> tarefaMapper)
        : IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO>
    {
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _tarefaMapper = tarefaMapper;

        public SolicitacaoObtencaoTarefaDTO MapToDto(SolicitacaoObtencaoTarefa solicitacao)
        {
            return new SolicitacaoObtencaoTarefaDTO
            {
                Id = solicitacao.Id,
                TarefaId = solicitacao.TarefaId,
                Status = (int)solicitacao.Status,
                DataSolicitacao = solicitacao.DataSolicitacao,
                DataDecisao = solicitacao.DataDecisao,
                Tarefa = solicitacao.Tarefa.MapToDto(_tarefaMapper),
                Solicitante = MapearUsuarioResumo(solicitacao.Solicitante),
                Aprovador = MapearUsuarioResumo(solicitacao.Aprovador)
            };
        }

        public IEnumerable<SolicitacaoObtencaoTarefaDTO> MapToDtos(
            IEnumerable<SolicitacaoObtencaoTarefa>? solicitacoes)
        {
            return solicitacoes?.Select(MapToDto) ?? [];
        }

        private static UsuarioDTO MapearUsuarioResumo(Usuario usuario)
        {
            return usuario is null
                ? null
                : new UsuarioDTO
                {
                    Id = usuario.Id,
                    Codigo = usuario.Codigo,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    Email = usuario.Email
                };
        }
    }
}
