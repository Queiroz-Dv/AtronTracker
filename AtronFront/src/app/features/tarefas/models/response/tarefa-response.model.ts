import { UsuarioResponse } from "../../../usuarios/models/response/usuario-response";
import { CargoModel } from "../../../cargos/models/cargo.model";
import { Departamento } from "../../../departamentos/models/departamento.model";
import { EstadoTarefa } from "../estadoTarefa.model";
import { TarefaModel } from "../tarefa.model";

export class TarefaResponse implements TarefaModel {
  id: number;
  identificador?: number;
  destinoInicial: number;
  exigeAprovacaoParaObter: boolean;
  titulo: string;
  conteudo: string;
  dataInicial: Date;
  dataFinal: Date;
  usuarioCodigo?: string;
  departamentoCodigo?: string;
  cargoCodigo?: string;

  usuario?: UsuarioResponse
  departamento?: Departamento
  cargo?: CargoModel
  estadoDaTarefa: EstadoTarefa
}
