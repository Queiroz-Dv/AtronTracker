using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class SolicitacaoObtencaoTarefaMapeador : ISolicitacaoObtencaoTarefaMapeador
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _tarefaMapeador;

        public SolicitacaoObtencaoTarefaMapeador(IAsyncApplicationMapService<TarefaDTO, Tarefa> tarefaMapeador)
        {
            _tarefaMapeador = tarefaMapeador;
        }

        public async Task<SolicitacaoObtencaoTarefaDTO> MapearAsync(SolicitacaoObtencaoTarefa solicitacao)
        {
            return new SolicitacaoObtencaoTarefaDTO
            {
                Id = solicitacao.Id,
                TarefaId = solicitacao.TarefaId,
                Status = solicitacao.Status,
                DataSolicitacao = solicitacao.DataSolicitacao,
                DataDecisao = solicitacao.DataDecisao,
                Tarefa = await _tarefaMapeador.MapToDTOAsync(solicitacao.Tarefa),
                Solicitante = MapearUsuarioResumo(solicitacao.Solicitante),
                Aprovador = MapearUsuarioResumo(solicitacao.Aprovador)
            };
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
