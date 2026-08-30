using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class EmpresaMapping : Mapper<Empresa, EmpresaDTO>, IUpdateMapper<Empresa, EmpresaDTO>
    {
        public override EmpresaDTO MapToDto(Empresa entity)
            => new()
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                NomeFantasia = entity.NomeFantasia,
                Endereco = entity.Endereco,
                Numero = entity.Numero,
                Email = entity.Email,
                Status = entity.Status
            };

        public override Empresa MapToEntity(EmpresaDTO dto)
            => new()
            {
                Codigo = NormalizarCodigo(dto.Codigo),
                NomeFantasia = dto.NomeFantasia.Trim(),
                Endereco = dto.Endereco.Trim(),
                Numero = dto.Numero.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Status = dto.Status
            };

        public void MapToUpdate(EmpresaDTO dto, Empresa entity)
        {
            entity.NomeFantasia = dto.NomeFantasia.Trim();
            entity.Endereco = dto.Endereco.Trim();
            entity.Numero = dto.Numero.Trim();
            entity.Email = dto.Email.Trim().ToLowerInvariant();
            entity.Status = dto.Status;
        }

        public static string NormalizarCodigo(string codigo)
            => codigo.Trim().ToUpperInvariant();
    }
}
