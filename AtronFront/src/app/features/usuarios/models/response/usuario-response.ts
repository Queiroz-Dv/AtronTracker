import { CargoModel } from "../../../cargos/models/cargo.model";
import { Departamento } from "../../../departamentos/models/departamento.model";
import { UsuarioRequest } from "../request/usuario-request";
import { UsuarioModel } from "../usuario.model";

export class UsuarioResponse implements UsuarioModel {
  codigo: string;
  nome: string;
  sobrenome: string;
  salario?: number;
  dataNascimento?: Date;
  email: string;
  senha: string;
  cargoCodigo: string;
  cargoDescricao: string;
  departamentoCodigo: string;
  departamentoDescricao: string;

  cargo: CargoModel
  departamento: Departamento
}

export function exibirDescricaoDoUsuario(usuario: UsuarioModel | UsuarioRequest | UsuarioResponse): string {
  return usuario ? usuario.nome + ' ' + usuario.sobrenome : '';
}