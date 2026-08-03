export interface PlanejamentoCustoModel {
  codigo: string;
  descricao: string;
  ano: number;
  valorMinimo: number;
  valorTeto: number;
  apenasDepartamento: boolean;
  departamentoCodigo: string;
  detalhesCargo: PlanejamentoCustoCargoModel[];
}

export interface PlanejamentoCustoCargoModel {
  cargoCodigo: string;
  cargoDescricao?: string;
  detalhado: boolean;
  valorMinimo?: number | null;
  valorTeto?: number | null;
}
