using Atron.Application.DTO;
using Atron.Application.DTO.Request;
using Atron.Application.DTO.Response;
using System.Linq;

namespace Atron.Application.Extensions
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

        #region Salario 
        public static SalarioDTO MontarDTO(this SalarioRequest salario)
        {
            return new SalarioDTO
            {
                UsuarioCodigo = salario.UsuarioCodigo,
                SalarioMensal = salario.SalarioMensal,
                MesId = salario.MesId,
                Ano = salario.Ano,
            };
        }

        public static SalarioResponse MontarResponse(this SalarioDTO salario)
        {
            return new SalarioResponse
            {
                Id = salario.Id,
                Ano = salario.Ano,
                SalarioMensal = salario.SalarioMensal,
                Mes = salario.Mes,
                Usuario = new UsuarioRecord
                {
                    UsuarioCodigo = salario.UsuarioCodigo,
                    Nome = salario.Usuario?.Nome,
                    Sobrenome = salario.Usuario?.Sobrenome,
                    CodigoCargo = salario.Usuario?.CargoCodigo,
                    DescricaoCargo = salario.Usuario?.Cargo?.Descricao,
                    CodigoDepartamento = salario.Usuario?.DepartamentoCodigo,
                    DescricaoDepartamento = salario.Usuario?.Departamento?.Descricao
                }
            };
        }
        #endregion

        #region Usuario
        public static UsuarioDTO MontarDTO(this UsuarioRequest usuario)
        {
            return new UsuarioDTO
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                DataNascimento = usuario.DataNascimento,
                Salario = usuario.SalarioMensal,
                CargoCodigo = usuario.CargoCodigo,
                DepartamentoCodigo = usuario.DepartamentoCodigo,
                PerfilDeAcessoCodigo = usuario.CodigoPerfilDeAcesso,
                Email = usuario.Email,
                Senha = usuario.Senha
            };
        }

        public static UsuarioResponse MontarResponse(this UsuarioDTO usuario)
        {
            return new UsuarioResponse
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                DataNascimento = usuario.DataNascimento,
                Salario = usuario.Salario,
                CargoCodigo = usuario.CargoCodigo,
                CargoDescricao = usuario.Cargo?.Descricao,
                DepartamentoCodigo = usuario.DepartamentoCodigo,
                DepartamentoDescricao = usuario.Departamento?.Descricao,
                PerfisDeAcesso = usuario.PerfisDeAcesso?.Select(p => new PerfilDeAcessoDTO
                {
                    Codigo = p.Codigo,
                    Descricao = p.Descricao
                }).ToList(),
                Email = usuario.Email,

            };
        }
        #endregion
    }
}