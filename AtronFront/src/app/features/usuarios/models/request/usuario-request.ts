import { UsuarioModel } from "../usuario.model";

export class UsuarioRequest implements UsuarioModel {
  codigo: string;
  nome: string;
  sobrenome: string;
  dataNascimento?: Date | string;
  email: string;
  senha?: string;
  clientUri?: string;
  cargoCodigo: string;
  departamentoCodigo: string;

  constructor(
    codigo: string,
    nome: string,
    sobrenome: string,
    email: string,
    cargoCodigo: string,
    departamentoCodigo: string,
    clientUri?: string,
    dataNascimento?: Date | string) {
    this.codigo = codigo;
    this.nome = nome;
    this.sobrenome = sobrenome;
    this.email = email;
    this.cargoCodigo = cargoCodigo;
    this.departamentoCodigo = departamentoCodigo;
    this.clientUri = clientUri;
    this.dataNascimento = dataNascimento
  }
}
