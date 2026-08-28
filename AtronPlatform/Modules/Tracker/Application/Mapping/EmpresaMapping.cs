using Application.DTO.Request;
using System.Collections.Generic;
using System.Linq;
using Application.DTO.Response;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class EmpresaMapping :
        IToEntityMapper<Empresa, EmpresaCadastroRequest>,
        IToDtoMapper<UsuarioEmpresa, EmpresaResponse>
    {
        public Empresa MapToEntity(EmpresaCadastroRequest request)
            => new()
            {
                Codigo = request.Codigo,
                NomeFantasia = request.NomeFantasia,
                Endereco = new Endereco { Logradouro = request.Endereco.Logradouro },
                Numero = request.Numero,
                Email = request.Email
            };

        public EmpresaResponse MapToDto(UsuarioEmpresa vinculo)
        {
            var empresa = vinculo.Empresa;
            return new EmpresaResponse(
                empresa.Id, empresa.Codigo, empresa.NomeFantasia,
                new EnderecoEmpresaResponse(empresa.Endereco.Logradouro),
                empresa.Numero, empresa.Email, empresa.Status, vinculo.Papel);
        }

        public IEnumerable<Empresa> MapToEntities(IEnumerable<EmpresaCadastroRequest>? requests)
            => requests?.Select(MapToEntity).ToArray() ?? [];

        public IEnumerable<EmpresaResponse> MapToDtos(IEnumerable<UsuarioEmpresa>? vinculos)
            => vinculos?.Select(MapToDto).ToArray() ?? [];
    }
}
