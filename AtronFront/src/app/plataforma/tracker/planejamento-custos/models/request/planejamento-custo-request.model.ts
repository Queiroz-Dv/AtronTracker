import { PlanejamentoCustoCargoModel, PlanejamentoCustoModel } from '../planejamento-custo.model';

export class PlanejamentoCustoCargoRequest implements PlanejamentoCustoCargoModel {
  cargoCodigo: string;
  detalhado: boolean;
  valorMinimo?: number | null;
  valorTeto?: number | null;

  constructor(cargoCodigo: string, detalhado: boolean, valorMinimo?: number | null, valorTeto?: number | null) {
    this.cargoCodigo = cargoCodigo;
    this.detalhado = detalhado;
    this.valorMinimo = valorMinimo;
    this.valorTeto = valorTeto;
  }
}

export class PlanejamentoCustoRequest implements PlanejamentoCustoModel {
  codigo: string;
  descricao: string;
  ano: number;
  valorMinimo: number;
  valorTeto: number;
  apenasDepartamento: boolean;
  departamentoCodigo: string;
  detalhesCargo: PlanejamentoCustoCargoRequest[];

  constructor(
    codigo: string,
    descricao: string,
    ano: number,
    valorMinimo: number,
    valorTeto: number,
    apenasDepartamento: boolean,
    departamentoCodigo: string,
    detalhesCargo: PlanejamentoCustoCargoRequest[] = []
  ) {
    this.codigo = codigo;
    this.descricao = descricao;
    this.ano = ano;
    this.valorMinimo = valorMinimo;
    this.valorTeto = valorTeto;
    this.apenasDepartamento = apenasDepartamento;
    this.departamentoCodigo = departamentoCodigo;
    this.detalhesCargo = detalhesCargo;
  }
}
