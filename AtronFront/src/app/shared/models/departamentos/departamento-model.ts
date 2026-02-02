import { CodigoDescricao } from "../types/codigo-descricao";

export class DepartamentoModel {
  constructor(public codigoDescricao: CodigoDescricao) { }

  estaValido(): boolean {
    return this.codigoDescricao.codigo != null && this.codigoDescricao.descricao != null;
  }

  toString() {
    return this.codigoDescricao.codigo + " - " + this.codigoDescricao.descricao
  }
}