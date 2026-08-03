using Application.DTO;
using Application.DTO.Request;
using Application.DTO.Response;

namespace Application.Extensions
{
    public static class BuilderExtensions
    {
        #region Cargo
        public static CargoDTO MontarDTO(this CargoRequest request) => new(request?.Codigo, request?.Descricao, request?.DepartamentoCodigo);
        public static CargoResponse MontarResponse(this CargoDTO dto) => new(dto?.Codigo, dto?.Descricao, dto?.DepartamentoCodigo, dto?.DepartamentoDescricao);
        #endregion

        #region Tarefa
        public static TarefaDTO MontarDTO(this TarefaRequest tarefa)
        {
            return new TarefaDTO
            {
                UsuarioCodigo = tarefa.UsuarioCodigo,
                Titulo = tarefa.Titulo,
                Conteudo = tarefa.Conteudo,
                DataInicial = tarefa.DataInicial,
                DataFinal = tarefa.DataFinal,
                EstadoDaTarefa = new TarefaEstadoDTO() { Id = tarefa.TarefaEstadoId }
            };
        }

        public static TarefaResponse MontarResponse(this TarefaDTO tarefa)
        {
            return new TarefaResponse
            {
                Id = tarefa.Id,
                Titulo = tarefa.Titulo,
                Conteudo = tarefa.Conteudo,
                DataInicial = tarefa.DataInicial,
                DataFinal = tarefa.DataFinal,
                EstadoDaTarefa = tarefa.EstadoDaTarefa,
                Usuario = new UsuarioRecord
                {
                    UsuarioCodigo = tarefa.UsuarioCodigo,
                    Nome = tarefa.Usuario?.Nome,
                    Sobrenome = tarefa.Usuario?.Sobrenome,
                    CodigoCargo = tarefa.Usuario?.CargoCodigo,
                    DescricaoCargo = tarefa.Usuario?.Cargo?.Descricao,
                    CodigoDepartamento = tarefa.Usuario?.DepartamentoCodigo,
                    DescricaoDepartamento = tarefa.Usuario?.Departamento?.Descricao
                }
            };
        }
        #endregion
    }
}
