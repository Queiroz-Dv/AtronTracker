import { UsuarioResponse } from "../../../usuarios/models/response/usuario-response";
import { TarefaResponse } from "./tarefa-response.model";

export class SolicitacaoObtencaoTarefaResponse {
  id: number;
  tarefaId: number;
  status: number;
  dataSolicitacao: Date;
  dataDecisao?: Date;
  tarefa: TarefaResponse;
  solicitante?: UsuarioResponse;
  aprovador?: UsuarioResponse;
}
