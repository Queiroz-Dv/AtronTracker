export interface UsuarioModel {
  codigo: string;
  nome: string;
  sobrenome: string;
  email: string;
  dataNascimento?: Date | string;
  senha?: string;
  cargoCodigo: string;
  departamentoCodigo: string;
}
