using Application.DTO;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    internal sealed class PlanejamentoCustoRelatorioHtmlMontador
    {
        private static readonly CultureInfo CulturaPtBr = new("pt-BR");

        public string Montar(PlanejamentoCustoRelatorioGeralDTO relatorio)
        {
            var departamentosHtml = string.Join(string.Empty, relatorio.Departamentos.Select(MontarDepartamento));
            var dataEmissao = DateTime.Today.ToString("dd 'de' MMMM 'de' yyyy", CulturaPtBr);

            return $@"
                    <!doctype html>
                    <html lang=""pt-BR"">
                      <head>
                        <meta charset=""utf-8"">
                        <title></title>
                        <style>
                          * {{ box-sizing: border-box; }}
                          body {{ background: #f6f3fb; color: #111827; font-family: Arial, sans-serif; margin: 0; padding: 28px; }}
                          header {{ background: linear-gradient(135deg, #312e81, #6d28d9); border-radius: 12px; color: #ffffff; margin-bottom: 18px; padding: 22px 24px; }}
                          h1 {{ font-size: 25px; margin: 0 0 8px; }}
                          h2 {{ color: #1f2937; font-size: 18px; margin: 0 0 4px; }}
                          h3 {{ font-size: 15px; margin: 0; }}
                          p {{ margin: 0; }}
                          .header-meta {{ color: #ede9fe; display: flex; flex-wrap: wrap; gap: 10px 18px; }}
                          .meta {{ color: #6b7280; }}
                          article {{ background: #ffffff; border: 1px solid #ddd6fe; border-radius: 10px; box-shadow: 0 8px 20px rgba(49, 46, 129, 0.07); }}
                          .department-list {{ display: grid; gap: 14px; }}
                          article {{ break-inside: avoid; display: grid; gap: 12px; padding: 16px; }}
                          .department-header {{ border-bottom: 1px solid #ede9fe; display: grid; grid-template-columns: minmax(0, 1fr) auto; gap: 16px; padding-bottom: 12px; }}
                          .department-title {{ min-width: 0; }}
                          .department-title h2, .department-title .meta {{ overflow-wrap: anywhere; }}
                          .values {{ align-content: start; display: grid; gap: 8px; grid-template-columns: repeat(2, max-content); justify-content: end; }}
                          .values span {{ background: #f5f3ff; border: 1px solid #ddd6fe; border-radius: 8px; color: #312e81; padding: 6px 10px; white-space: nowrap; }}
                          .pending {{ background: #fffbeb; border: 1px solid #fcd34d; border-radius: 8px; color: #78350f; display: grid; gap: 4px; padding: 10px 12px; }}
                          table {{ border-collapse: collapse; table-layout: fixed; width: 100%; }}
                          th, td {{ border-bottom: 1px solid #ede9fe; padding: 10px 12px; text-align: left; }}
                          th {{ background: #ede9fe; color: #312e81; font-weight: 700; }}
                          th:nth-child(1), td:nth-child(1) {{ width: 52%; }}
                          th:nth-child(2), td:nth-child(2),
                          th:nth-child(3), td:nth-child(3),
                          th:nth-child(4), td:nth-child(4) {{ white-space: nowrap; width: 16%; }}
                          tbody tr:nth-child(even) {{ background: #faf9ff; }}
                          @media (max-width: 720px) {{
                            .department-header {{ grid-template-columns: 1fr; }}
                            .values {{ justify-content: start; }}
                          }}
                          @media print {{
                            @page {{ margin: 0; }}
                            body {{ background: #ffffff; color: #111827; padding: 14mm; }}
                            header {{ background: #ffffff; border: 1px solid #111827; color: #111827; }}
                            .header-meta {{ color: #111827; }}
                            article {{ box-shadow: none; }}
                            article {{ break-inside: avoid; }}
                          }}
                        </style>
                      </head>
                      <body>
                        <header>
                          <h1>Relatorio de Planejamentos</h1>
                          <div class=""header-meta"">
                            <span>Ano analisado: {Escapar(relatorio.Ano.ToString(CulturaPtBr))}</span>
                            <span>Emitido em {Escapar(dataEmissao)}</span>
                          </div>
                        </header>
                        <section class=""department-list"" aria-label=""Registros do relatorio"">
                          {departamentosHtml.NullIfEmpty() ?? "<p>Nenhum planejamento encontrado para o ano informado.</p>"}
                        </section>
                      </body>
                    </html>";
        }

        private static string MontarDepartamento(PlanejamentoCustoRelatorioDepartamentoDTO departamento)
        {
            var pendenciasHtml = departamento.CargosPendentes.Any()
                ? $@"<div class=""pending""><strong>Cargos pendentes</strong><span>{Escapar(string.Join(", ", departamento.CargosPendentes))}</span></div>"
                : string.Empty;
            var cargosHtml = departamento.CargosDetalhados.Any()
                ? MontarCargos(departamento)
                : string.Empty;

            return $@"
      <article>
        <div class=""department-header"">
          <div class=""department-title"">
            <p class=""meta"">{Escapar(departamento.DepartamentoCodigo)} - {Escapar(departamento.DepartamentoDescricao)}</p>
            <h2>{Escapar(departamento.PlanejamentoCodigo)} - {Escapar(departamento.PlanejamentoDescricao)}</h2>
          </div>
          <div class=""values"">
            <span>Minimo: {FormatarMoeda(departamento.ValorMinimoDepartamento ?? 0)}</span>
            <span>Teto: {FormatarMoeda(departamento.ValorTetoDepartamento ?? 0)}</span>
          </div>
        </div>
        {pendenciasHtml}
        {cargosHtml}
      </article>";
        }

        private static string MontarCargos(PlanejamentoCustoRelatorioDepartamentoDTO departamento)
        {
            var linhas = new StringBuilder();
            foreach (var cargo in departamento.CargosDetalhados)
            {
                linhas.Append($@"
          <tr>
            <td>{Escapar(cargo.CargoCodigo)} - {Escapar(cargo.CargoDescricao)}</td>
            <td>{FormatarMoeda(cargo.ValorMinimo)}</td>
            <td>{FormatarMoeda(cargo.ValorTeto)}</td>
            <td>{FormatarNumero(cargo.PercentualOcupacaoTetoDepartamento)}%</td>
          </tr>");
            }

            return $@"
        <table>
          <thead>
            <tr>
              <th>Cargo</th>
              <th>Minimo</th>
              <th>Teto</th>
              <th>% do teto</th>
            </tr>
          </thead>
          <tbody>{linhas}</tbody>
        </table>";
        }

        private static string FormatarMoeda(decimal valor)
        {
            return valor.ToString("C", CulturaPtBr);
        }

        private static string FormatarNumero(decimal valor)
        {
            return valor.ToString("0.##", CulturaPtBr);
        }

        private static string Escapar(string valor)
        {
            return WebUtility.HtmlEncode(valor ?? string.Empty);
        }
    }

    internal static class StringExtensions
    {
        public static string NullIfEmpty(this string valor)
        {
            return string.IsNullOrEmpty(valor) ? null : valor;
        }
    }
}
