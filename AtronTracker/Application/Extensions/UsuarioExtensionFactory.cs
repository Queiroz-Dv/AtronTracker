using Application.DTO;
using Application.DTO.Request;
using Application.DTO.Response;
using System.Collections.Generic;
using System.Linq;

namespace Application.Extensions
{
    public static class UsuarioExtensionFactory
    {
        public static UsuarioDTO MontarUsuarioDTO(this UsuarioRequest usuario)
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

        public static UsuarioResponse MontarUsuarioResponse(this UsuarioDTO usuario)
        {
            return new UsuarioResponse
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                DataNascimento = usuario.DataNascimento?.ToString("dd/MM/yyyy"),
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

        public static IEnumerable<UsuarioResponse> MontarUsuarioResponse(this IList<UsuarioDTO> usuariosDTO)
        {
            var usuariosResponse = new List<UsuarioResponse>();

            foreach (var usuario in usuariosDTO)
            {
                var response = new UsuarioResponse
                {
                    Codigo = usuario.Codigo,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    DataNascimento = usuario.DataNascimento?.ToString("dd/MM/yyyy"),
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

                usuariosResponse.Add(response);
            }

            return usuariosResponse;
        }
    }
}
