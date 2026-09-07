using Application.DTO.Request;
using Domain.Entities;
using Domain.ValueObjects;
using Shared.Application.Interfaces.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace Application.Mapping
{
    public sealed class EmpresaMapping :
        IToEntityMapper<Empresa, EmpresaCadastroRequest>
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


        public IEnumerable<Empresa> MapToEntities(IEnumerable<EmpresaCadastroRequest>? requests)
            => requests?.Select(MapToEntity).ToArray() ?? [];
    }
}