import { EstadoTarefa } from "./estadoTarefa.model";

export interface TarefaModel {
  id: number;
  titulo: string;
  conteudo: string;
  dataInicial: Date;
  dataFinal: Date;

  usuarioCodigo: string;
  estadoDaTarefa: EstadoTarefa;
}