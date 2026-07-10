import { SalarioModel } from "../salario.model";

export class SalarioRequest implements SalarioModel {
  id: number;
  salarioMensal: number;
  usuarioCodigo: string;
  mesId: number;
  ano: string;
}