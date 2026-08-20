using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;
using System.Linq;

namespace Application.Mapping
{
    public class PlanejamentoCustoMapping : Mapper<PlanejamentoCusto, PlanejamentoCustoDTO>
    {
        public override PlanejamentoCustoDTO MapToDto(PlanejamentoCusto entity)
        {
            return new PlanejamentoCustoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                Ano = entity.Ano,
                ValorMinimo = entity.ValorMinimo,
                ValorTeto = entity.ValorTeto,
                ApenasDepartamento = entity.ApenasDepartamento,
                DepartamentoId = entity.DepartamentoId,
                DepartamentoCodigo = entity.DepartamentoCodigo,
                DepartamentoDescricao = entity.Departamento?.Descricao,
                Departamento = entity.Departamento != null ? new DepartamentoDTO
                {
                    Id = entity.Departamento.Id,
                    Codigo = entity.Departamento.Codigo,
                    Descricao = entity.Departamento.Descricao
                } : null,
                DetalhesCargo = entity.DetalhesCargo?.Select(detalhe => new PlanejamentoCustoCargoDTO
                {
                    Id = detalhe.Id,
                    CargoId = detalhe.CargoId,
                    CargoCodigo = detalhe.CargoCodigo,
                    CargoDescricao = detalhe.Cargo?.Descricao,
                    Detalhado = detalhe.Detalhado,
                    ValorMinimo = detalhe.ValorMinimo,
                    ValorTeto = detalhe.ValorTeto
                }).ToList() ?? []
            };

        }

        public override PlanejamentoCusto MapToEntity(PlanejamentoCustoDTO dto)
        {
            return new PlanejamentoCusto
            {
                Id = dto.Id,
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                Ano = dto.Ano,
                ValorMinimo = dto.ValorMinimo,
                ValorTeto = dto.ValorTeto,
                ApenasDepartamento = dto.ApenasDepartamento,
                DepartamentoCodigo = dto.DepartamentoCodigo,
                DetalhesCargo = dto.DetalhesCargo?.Select(detalhe => new PlanejamentoCustoCargo
                {
                    CargoId = detalhe.CargoId,
                    CargoCodigo = detalhe.CargoCodigo,
                    Detalhado = detalhe.Detalhado,
                    ValorMinimo = detalhe.ValorMinimo,
                    ValorTeto = detalhe.ValorTeto
                }).ToList() ?? []
            };

        }

        public void MapToEntity(PlanejamentoCustoDTO dto, PlanejamentoCusto entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.ValorMinimo = dto.ValorMinimo;
            entityToUpdate.ValorTeto = dto.ValorTeto;
            entityToUpdate.ApenasDepartamento = dto.ApenasDepartamento;
        }
    }
}
