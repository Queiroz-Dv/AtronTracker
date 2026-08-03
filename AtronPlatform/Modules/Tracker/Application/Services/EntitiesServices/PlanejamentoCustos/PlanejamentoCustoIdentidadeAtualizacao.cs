using Application.DTO;
using Application.Resources;
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
                return Resultado.Falha(PlanejamentoCustoResource.Erro_CodigoNaoPodeSerAlterado);

            if (dto.Ano != planejamento.Ano)
                return Resultado.Falha(PlanejamentoCustoResource.Erro_AnoNaoPodeSerAlterado);

            if (!dto.DepartamentoCodigo.IsNullOrEmpty() &&
                dto.DepartamentoCodigo != planejamento.DepartamentoCodigo)
                return Resultado.Falha(PlanejamentoCustoResource.Erro_DepartamentoNaoPodeSerAlterado);

            dto.Codigo = planejamento.Codigo;
            dto.Id = planejamento.Id;
            dto.Ano = planejamento.Ano;
            dto.DepartamentoId = planejamento.DepartamentoId;
            dto.DepartamentoCodigo = planejamento.DepartamentoCodigo;

            return Resultado.Sucesso();
        }
    }
}