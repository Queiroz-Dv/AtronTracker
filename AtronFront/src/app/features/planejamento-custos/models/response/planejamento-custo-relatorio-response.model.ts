export interface PlanejamentoCustoRelatorioCargoResponse {
  cargoCodigo: string;
  cargoDescricao: string;
  valorMinimo: number;
  valorTeto: number;
  percentualOcupacaoTetoDepartamento: number;
}

export interface PlanejamentoCustoRelatorioDepartamentoResponse {
  departamentoCodigo: string;
  departamentoDescricao: string;
  possuiPlanejamento: boolean;
  planejamentoCodigo?: string;
  planejamentoDescricao?: string;
  apenasDepartamento: boolean;
  valorMinimoDepartamento?: number | null;
  valorTetoDepartamento?: number | null;
  somaMinimosCargos: number;
  somaTetosCargos: number;
  percentualOcupacaoTeto?: number | null;
  quantidadeCargosNaoDetalhados: number;
  cargosDetalhados: PlanejamentoCustoRelatorioCargoResponse[];
  cargosPendentes: string[];
  informacoes: string[];
}

export interface PlanejamentoCustoRelatorioGeralResponse {
  ano: number;
  departamentos: PlanejamentoCustoRelatorioDepartamentoResponse[];
}
