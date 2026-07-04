export class PlanejamentoCustoCargoResponse {
  cargoCodigo: string;
  cargoDescricao: string;
  detalhado: boolean;
  valorMinimo?: number | null;
  valorTeto?: number | null;
}

export class PlanejamentoCustoResponse {
  id: number;
  codigo: string;
  descricao: string;
  ano: number;
  valorMinimo: number;
  valorTeto: number;
  apenasDepartamento: boolean;
  departamentoCodigo: string;
  departamentoDescricao: string;
  detalhesCargo: PlanejamentoCustoCargoResponse[];
}
