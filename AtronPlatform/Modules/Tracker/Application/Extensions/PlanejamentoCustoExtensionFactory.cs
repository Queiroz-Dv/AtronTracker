using Application.DTO;
using Application.DTO.Request;
using Application.DTO.Response;
using System.Linq;

namespace Application.Extensions
{
    public static class PlanejamentoCustoExtensionFactory
    {
        public static PlanejamentoCustoDTO MontarDTO(this PlanejamentoCustoRequest request)
        {
            return new PlanejamentoCustoDTO
            {
                Codigo = request.Codigo?.ToUpperInvariant(),
                Descricao = request.Descricao,
                Ano = request.Ano,
                ValorMinimo = request.ValorMinimo,
                ValorTeto = request.ValorTeto,
                ApenasDepartamento = request.ApenasDepartamento,
                DepartamentoCodigo = request.DepartamentoCodigo?.ToUpperInvariant(),
                DetalhesCargo = request.ApenasDepartamento ? [] : request.DetalhesCargo?.Select(detalhe => new PlanejamentoCustoCargoDTO
                {
                    CargoCodigo = detalhe.CargoCodigo?.ToUpperInvariant(),
                    Detalhado = detalhe.Detalhado,
                    ValorMinimo = detalhe.ValorMinimo,
                    ValorTeto = detalhe.ValorTeto
                }).ToList() ?? []
            };
        }

        public static PlanejamentoCustoResponse MontarResponse(this PlanejamentoCustoDTO dto)
        {
            return new PlanejamentoCustoResponse
            {
                Id = dto.Id,
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                Ano = dto.Ano,
                ValorMinimo = dto.ValorMinimo,
                ValorTeto = dto.ValorTeto,
                ApenasDepartamento = dto.ApenasDepartamento,
                DepartamentoCodigo = dto.DepartamentoCodigo,
                DepartamentoDescricao = dto.DepartamentoDescricao,
                DetalhesCargo = dto.DetalhesCargo?.Select(detalhe => new PlanejamentoCustoCargoResponse
                {
                    CargoCodigo = detalhe.CargoCodigo,
                    CargoDescricao = detalhe.CargoDescricao,
                    Detalhado = detalhe.Detalhado,
                    ValorMinimo = detalhe.ValorMinimo,
                    ValorTeto = detalhe.ValorTeto
                }).ToList() ?? []
            };
        }

        public static PlanejamentoCustoRelatorioGeralResponse MontarResponse(this PlanejamentoCustoRelatorioGeralDTO dto)
        {
            return new PlanejamentoCustoRelatorioGeralResponse
            {
                Ano = dto.Ano,
                Departamentos = dto.Departamentos?.Select(departamento => new PlanejamentoCustoRelatorioDepartamentoResponse
                {
                    DepartamentoCodigo = departamento.DepartamentoCodigo,
                    DepartamentoDescricao = departamento.DepartamentoDescricao,
                    PossuiPlanejamento = departamento.PossuiPlanejamento,
                    PlanejamentoCodigo = departamento.PlanejamentoCodigo,
                    PlanejamentoDescricao = departamento.PlanejamentoDescricao,
                    ApenasDepartamento = departamento.ApenasDepartamento,
                    ValorMinimoDepartamento = departamento.ValorMinimoDepartamento,
                    ValorTetoDepartamento = departamento.ValorTetoDepartamento,
                    SomaMinimosCargos = departamento.SomaMinimosCargos,
                    SomaTetosCargos = departamento.SomaTetosCargos,
                    PercentualOcupacaoTeto = departamento.PercentualOcupacaoTeto,
                    QuantidadeCargosNaoDetalhados = departamento.QuantidadeCargosNaoDetalhados,
                    CargosPendentes = departamento.CargosPendentes,
                    Informacoes = departamento.Informacoes,
                    CargosDetalhados = departamento.CargosDetalhados?.Select(cargo => new PlanejamentoCustoRelatorioCargoResponse
                    {
                        CargoCodigo = cargo.CargoCodigo,
                        CargoDescricao = cargo.CargoDescricao,
                        ValorMinimo = cargo.ValorMinimo,
                        ValorTeto = cargo.ValorTeto,
                        PercentualOcupacaoTetoDepartamento = cargo.PercentualOcupacaoTetoDepartamento
                    }).ToList() ?? []
                }).ToList() ?? []
            };
        }
    }
}
