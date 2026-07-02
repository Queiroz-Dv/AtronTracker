import { EstadoTarefa } from "./estadoTarefa.model";

export interface TarefaModel {
  id: number;
  titulo: string;
  conteudo: string;
  dataInicial: Date | string;
  dataFinal: Date | string;

  usuarioCodigo: string;
  estadoDaTarefa: EstadoTarefa;
}
