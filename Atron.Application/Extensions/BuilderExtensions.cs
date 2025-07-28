using Atron.Application.DTO;
using Atron.Application.DTO.Request;
using Atron.Application.DTO.Response;
using Atron.Domain.Entities;

namespace Atron.Application.Extensions
{
    public static class BuilderExtensions
    {
        public static TarefaDTO MontarDTO(this TarefaRequest tarefa)
        {
            return new TarefaDTO
            {
                UsuarioCodigo = tarefa.UsuarioCodigo,
                Titulo = tarefa.Titulo,
                Conteudo = tarefa.Conteudo,
                DataInicial = tarefa.DataInicial,
                DataFinal = tarefa.DataFinal,
                EstadoDaTarefa = new TarefaEstado() { Id = tarefa.TarefaEstadoId }
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
    }
}