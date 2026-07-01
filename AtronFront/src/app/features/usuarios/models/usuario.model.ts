export interface UsuarioModel {
  codigo: string;
  nome: string;
  sobrenome: string;
  email: string;
  salario?: number;
  dataNascimento?: Date | string;
  senha: string;
  cargoCodigo: string;
  departamentoCodigo: string;
}
