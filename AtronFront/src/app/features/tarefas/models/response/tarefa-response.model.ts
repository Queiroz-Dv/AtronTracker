import { UsuarioResponse } from "../../../usuarios/models/response/usuario-response";
import { EstadoTarefa } from "../estadoTarefa.model";
import { TarefaModel } from "../tarefa.model";

export class TarefaResponse implements TarefaModel {
  id: number;
  titulo: string;
  conteudo: string;
  dataInicial: Date;
  dataFinal: Date;
  usuarioCodigo: string;

  usuario: UsuarioResponse
  estadoDaTarefa: EstadoTarefa
}