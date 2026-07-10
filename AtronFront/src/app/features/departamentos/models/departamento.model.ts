import { DepartamentoItem } from "../interfaces/departamento-item.interface";

export class Departamento implements DepartamentoItem {

  constructor(
    public codigo: string,
    public descricao: string,
    public gestorDepartamentoCodigo?: string,
    public gestorDepartamentoNome?: string
  ) { }

  estaValido(): boolean {
    return this.codigo != null && this.descricao != null;
  }

  toString() {
    return this.codigo + " - " + this.descricao
  }
}
