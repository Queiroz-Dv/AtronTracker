using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    internal sealed class PlanejamentoCustoIdentidadeAtualizacao
    {
        public Resultado Aplicar(PlanejamentoCusto planejamento, PlanejamentoCustoDTO dto)
        {
            if (!dto.Codigo.IsNullOrEmpty() &&
                dto.Codigo != planejamento.Codigo)
                return Resultado.Falha("O codigo do planejamento de custo nao pode ser alterado.");

            if (dto.Ano != planejamento.Ano)
                return Resultado.Falha("O ano do planejamento de custo nao pode ser alterado.");

            if (!dto.DepartamentoCodigo.IsNullOrEmpty() &&
                dto.DepartamentoCodigo != planejamento.DepartamentoCodigo)
                return Resultado.Falha("O departamento do planejamento de custo nao pode ser alterado.");

            dto.Codigo = planejamento.Codigo;
            dto.Id = planejamento.Id;
            dto.Ano = planejamento.Ano;
            dto.DepartamentoId = planejamento.DepartamentoId;
            dto.DepartamentoCodigo = planejamento.DepartamentoCodigo;

            return Resultado.Sucesso();
        }
    }
}
