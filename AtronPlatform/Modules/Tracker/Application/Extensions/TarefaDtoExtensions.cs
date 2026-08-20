using Application.DTO;
using Application.Resources;
using Shared.Extensions;

namespace Application.Extensions
{
    public static class TarefaDtoExtensions
    {
        public static string ObterDescricaoEstado(this TarefaDTO tarefa)
        {
            return tarefa.EstadoDaTarefa is not null && !tarefa.EstadoDaTarefa.Descricao.IsNullOrEmpty()
                ? tarefa.EstadoDaTarefa.Descricao
                : TarefaResource.Descricao_EstadoNaoInformado;
        }
    }
}