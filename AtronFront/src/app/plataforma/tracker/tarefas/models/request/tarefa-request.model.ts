import { EstadoTarefa } from "../estadoTarefa.model";
import { TarefaModel } from "../tarefa.model";

export class TarefaRequest implements TarefaModel {
  id: number;
  destinoInicial: number;
  exigeAprovacaoParaObter: boolean;
  titulo: string;
  conteudo: string;
  dataInicial: Date | string;
  dataFinal: Date | string;
  usuarioCodigo?: string;
  departamentoCodigo?: string;
  cargoCodigo?: string;

  estadoDaTarefa: EstadoTarefa;
}
