import { EstadoTarefa } from "./estadoTarefa.model";

export interface TarefaModel {
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
