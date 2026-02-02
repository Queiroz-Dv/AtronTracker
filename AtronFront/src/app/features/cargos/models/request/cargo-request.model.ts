import { Departamento } from "../../../departamentos/models/departamento.model";
import { CargoModel } from "../cargo.model";

export class CargoRequest implements CargoModel {
  departamento: Departamento;
  codigo: string;
  descricao: string;
  departamentoCodigo: string;

  constructor(codigo: string, descricao: string, departamentoCodigo: string) {
    this.codigo = codigo;
    this.descricao = descricao;
    this.departamentoCodigo = departamentoCodigo;
  }
}