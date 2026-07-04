using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class PlanejamentoCustoMapping : AsyncApplicationMapService<PlanejamentoCustoDTO, PlanejamentoCusto>, IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto>
    {
        public override Task<PlanejamentoCustoDTO> MapToDTOAsync(PlanejamentoCusto entity)
        {
            var dto = new PlanejamentoCustoDTO
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

            return Task.FromResult(dto);
        }

        public override Task<PlanejamentoCusto> MapToEntityAsync(PlanejamentoCustoDTO dto)
        {
            var entity = new PlanejamentoCusto
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

            return Task.FromResult(entity);
        }

        public Task MapToEntityAsync(PlanejamentoCustoDTO dto, PlanejamentoCusto entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.ValorMinimo = dto.ValorMinimo;
            entityToUpdate.ValorTeto = dto.ValorTeto;
            entityToUpdate.ApenasDepartamento = dto.ApenasDepartamento;
            return Task.CompletedTask;
        }
    }
}
