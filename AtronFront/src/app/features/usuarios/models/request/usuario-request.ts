import { UsuarioModel } from "../usuario.model";

export class UsuarioRequest implements UsuarioModel {
  codigo: string;
  nome: string;
  sobrenome: string;
  salario?: number;
  dataNascimento?: Date | string;
  email: string;
  senha: string;
  cargoCodigo: string;
  departamentoCodigo: string;

  constructor(
    codigo: string,
    nome: string,
    sobrenome: string,
    email: string,
    senha: string,
    cargoCodigo: string,
    departamentoCodigo: string,
    salario?: number,
    dataNascimento?: Date | string) {
    this.codigo = codigo;
    this.nome = nome;
    this.sobrenome = sobrenome;
    this.email = email;
    this.senha = senha;
    this.cargoCodigo = cargoCodigo;
    this.departamentoCodigo = departamentoCodigo;
    this.salario = salario
    this.dataNascimento = dataNascimento
  }
}
