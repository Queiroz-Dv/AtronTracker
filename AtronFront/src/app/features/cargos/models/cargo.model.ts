import { Departamento } from "../../departamentos/models/departamento.model";

export interface CargoModel {
  codigo: string;
  descricao: string;
  departamentoCodigo: string;
  departamento: Departamento
}