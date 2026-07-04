import { Injectable } from '@angular/core';
import { Nivel, NotificacaoService } from '../../../core/services/notification.service';
import {
  PlanejamentoCustoRelatorioDepartamentoResponse,
  PlanejamentoCustoRelatorioGeralResponse
} from '../models/response/planejamento-custo-relatorio-response.model';

@Injectable({
  providedIn: 'root'
})
export class PlanejamentoCustoRelatorioImpressaoService {
  constructor(private notificacaoService: NotificacaoService) { }

  imprimir(relatorio: PlanejamentoCustoRelatorioGeralResponse): void {
    const iframe = document.createElement('iframe');
    iframe.style.position = 'fixed';
    iframe.style.right = '0';
    iframe.style.bottom = '0';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = '0';

    document.body.appendChild(iframe);
    const documento = iframe.contentDocument ?? iframe.contentWindow?.document;
    if (!documento || !iframe.contentWindow) {
      iframe.remove();
      this.notificacaoService.exibirMensagem('Nao foi possivel preparar o relatorio para impressao.', Nivel.Error);
      return;
    }

    documento.open();
    documento.write(this.obterHtmlRelatorio(relatorio));
    documento.close();

    iframe.onload = () => {
      iframe.contentWindow?.focus();
      iframe.contentWindow?.print();
      setTimeout(() => iframe.remove(), 1000);
    };
  }

  private obterHtmlRelatorio(relatorio: PlanejamentoCustoRelatorioGeralResponse): string {
    const totalDepartamentos = relatorio.departamentos.length;
    const totalCargosDetalhados = relatorio.departamentos.reduce((total, departamento) => total + departamento.cargosDetalhados.length, 0);
    const totalPlanejadoDepartamento = relatorio.departamentos.reduce((total, departamento) => total + Number(departamento.valorTetoDepartamento ?? 0), 0);
    const departamentosHtml = relatorio.departamentos.map(departamento => this.obterHtmlDepartamento(departamento)).join('');
    const dataEmissao = this.formatarDataAtual();

    return `
      <!doctype html>
      <html lang="pt-BR">
        <head>
          <meta charset="utf-8">
          <title></title>
          <style>
            * { box-sizing: border-box; }
            body { background: #f6f3fb; color: #111827; font-family: Arial, sans-serif; margin: 0; padding: 28px; }
            header { background: linear-gradient(135deg, #312e81, #6d28d9); border-radius: 12px; color: #ffffff; margin-bottom: 18px; padding: 22px 24px; }
            h1 { font-size: 25px; margin: 0 0 8px; }
            h2 { color: #1f2937; font-size: 18px; margin: 0 0 4px; }
            h3 { font-size: 15px; margin: 0; }
            p { margin: 0; }
            .header-meta { color: #ede9fe; display: flex; flex-wrap: wrap; gap: 10px 18px; }
            .meta { color: #6b7280; }
            .summary { display: grid; gap: 12px; grid-template-columns: repeat(3, minmax(150px, 1fr)); margin-bottom: 18px; }
            .summary div, article { background: #ffffff; border: 1px solid #ddd6fe; border-radius: 10px; box-shadow: 0 8px 20px rgba(49, 46, 129, 0.07); }
            .summary div { display: grid; gap: 4px; padding: 15px; }
            .summary strong { color: #4c1d95; font-size: 20px; }
            .summary span { color: #4b5563; }
            .department-list { display: grid; gap: 14px; }
            article { break-inside: avoid; display: grid; gap: 12px; padding: 16px; }
            .department-header { border-bottom: 1px solid #ede9fe; display: flex; align-items: flex-start; justify-content: space-between; gap: 16px; padding-bottom: 12px; }
            .values, .messages { display: flex; flex-wrap: wrap; gap: 8px; }
            .values span, .messages span { background: #f5f3ff; border: 1px solid #ddd6fe; border-radius: 8px; color: #312e81; padding: 6px 10px; }
            .pending { background: #fffbeb; border: 1px solid #fcd34d; border-radius: 8px; color: #78350f; display: grid; gap: 4px; padding: 10px 12px; }
            table { border-collapse: collapse; width: 100%; }
            th, td { border-bottom: 1px solid #ede9fe; padding: 10px 12px; text-align: left; }
            th { background: #ede9fe; color: #312e81; font-weight: 700; }
            tbody tr:nth-child(even) { background: #faf9ff; }
            @media print {
              @page { margin: 0; }
              body { background: #ffffff; color: #111827; padding: 14mm; }
              header { background: #ffffff; border: 1px solid #111827; color: #111827; }
              .header-meta { color: #111827; }
              .summary div, article { box-shadow: none; }
              article { break-inside: avoid; }
            }
          </style>
        </head>
        <body>
          <header>
            <h1>Relatorio de Planejamentos</h1>
            <div class="header-meta">
              <span>Ano analisado: ${this.escaparHtml(String(relatorio.ano))}</span>
              <span>Emitido em ${this.escaparHtml(dataEmissao)}</span>
            </div>
          </header>
          <section class="summary" aria-label="Resumo do relatorio">
            <div>
              <strong>${totalDepartamentos}</strong>
              <span>Departamentos planejados</span>
            </div>
            <div>
              <strong>${totalCargosDetalhados}</strong>
              <span>Cargos detalhados</span>
            </div>
            <div>
              <strong>${this.formatarMoeda(totalPlanejadoDepartamento)}</strong>
              <span>Teto planejado</span>
            </div>
          </section>
          <section class="department-list" aria-label="Registros do relatorio">
            ${departamentosHtml || '<p>Nenhum planejamento encontrado para o ano informado.</p>'}
          </section>
        </body>
      </html>`;
  }

  private obterHtmlDepartamento(departamento: PlanejamentoCustoRelatorioDepartamentoResponse): string {
    const informacoesHtml = departamento.informacoes.length
      ? `<div class="messages">${departamento.informacoes.map(informacao => `<span>${this.escaparHtml(informacao)}</span>`).join('')}</div>`
      : '';
    const pendenciasHtml = departamento.cargosPendentes.length
      ? `<div class="pending"><strong>Cargos pendentes</strong><span>${this.escaparHtml(departamento.cargosPendentes.join(', '))}</span></div>`
      : '';
    const cargosHtml = departamento.cargosDetalhados.length
      ? this.obterHtmlCargos(departamento)
      : '';

    return `
      <article>
        <div class="department-header">
          <div>
            <p class="meta">${this.escaparHtml(departamento.departamentoCodigo)} - ${this.escaparHtml(departamento.departamentoDescricao)}</p>
            <h2>${this.escaparHtml(departamento.planejamentoCodigo ?? '')} - ${this.escaparHtml(departamento.planejamentoDescricao ?? '')}</h2>
          </div>
          <div class="values">
            <span>Minimo: ${this.formatarMoeda(departamento.valorMinimoDepartamento ?? 0)}</span>
            <span>Teto: ${this.formatarMoeda(departamento.valorTetoDepartamento ?? 0)}</span>
            ${departamento.percentualOcupacaoTeto !== null && departamento.percentualOcupacaoTeto !== undefined
        ? `<span>${this.formatarNumero(departamento.percentualOcupacaoTeto)}% ocupado</span>`
        : ''}
          </div>
        </div>
        ${informacoesHtml}
        ${pendenciasHtml}
        ${cargosHtml}
      </article>`;
  }

  private obterHtmlCargos(departamento: PlanejamentoCustoRelatorioDepartamentoResponse): string {
    const linhas = departamento.cargosDetalhados.map(cargo => `
      <tr>
        <td>${this.escaparHtml(cargo.cargoCodigo)} - ${this.escaparHtml(cargo.cargoDescricao)}</td>
        <td>${this.formatarMoeda(cargo.valorMinimo)}</td>
        <td>${this.formatarMoeda(cargo.valorTeto)}</td>
        <td>${this.formatarNumero(cargo.percentualOcupacaoTetoDepartamento)}%</td>
      </tr>`).join('');

    return `
      <table>
        <thead>
          <tr>
            <th>Cargo</th>
            <th>Minimo</th>
            <th>Teto</th>
            <th>% do teto</th>
          </tr>
        </thead>
        <tbody>${linhas}</tbody>
      </table>`;
  }

  private formatarMoeda(valor: number): string {
    return Number(valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }

  private formatarNumero(valor: number): string {
    return Number(valor ?? 0).toLocaleString('pt-BR', { minimumFractionDigits: 0, maximumFractionDigits: 2 });
  }

  private formatarDataAtual(): string {
    return new Date().toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  private escaparHtml(valor: string): string {
    return valor
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }
}
