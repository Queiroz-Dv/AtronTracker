import { EstadoTarefa } from "../estadoTarefa.model";
import { TarefaModel } from "../tarefa.model";

export class TarefaRequest implements TarefaModel {
  id: number;
  titulo: string;
  conteudo: string;
  dataInicial: Date | string;
  dataFinal: Date | string;
  usuarioCodigo: string;

  estadoDaTarefa: EstadoTarefa;
}
