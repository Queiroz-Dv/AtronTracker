import { MesDto } from "../../../models/mes.dto";
import { UsuarioResponse } from "../../../usuarios/models/response/usuario-response";

export class SalarioResponse {
  id: number;
  salarioMensal: number;
  usuarioId: number;
  usuarioCodigo: string;
  mesId: number;
  ano: string;

  usuario: UsuarioResponse;
  mes: MesDto;
}