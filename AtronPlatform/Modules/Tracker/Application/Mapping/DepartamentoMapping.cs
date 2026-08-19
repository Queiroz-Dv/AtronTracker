using Application.DTO;
using Domain.Entities;
using Domain.Extensions;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class DepartamentoMapping : Mapper<Departamento, DepartamentoDTO>
    {
        public override DepartamentoDTO MapToDto(Departamento entity)
        {
            return new DepartamentoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo.ToUpper(),
                Descricao = entity.Descricao.ToUpper(),
                GestorDepartamentoCodigo = entity.GestorDepartamentoCodigo,
                GestorDepartamentoNome = entity.GestorDepartamento.ObterNome()
            };
        }

        public override Departamento MapToEntity(DepartamentoDTO dto)
        {
            return new Departamento
            {
                Id = dto.Id,
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                GestorDepartamentoCodigo = dto.GestorDepartamentoCodigo?.ToUpper()
            };
        }

        public static void MapToEntity(DepartamentoDTO dto, Departamento entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.GestorDepartamentoCodigo = dto.GestorDepartamentoCodigo?.ToUpper();
        }        
    }
}
