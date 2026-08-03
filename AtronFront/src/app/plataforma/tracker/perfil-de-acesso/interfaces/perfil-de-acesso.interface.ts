import { ModuloModel } from "../../../../features/navegacao/modulos/interfaces/modulo.interface";

export interface PerfilDeAcessoModel {
  id?: number,
  codigo: string;
  descricao: string;

  modulos: ModuloModel[];
}