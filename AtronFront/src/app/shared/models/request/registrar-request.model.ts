export enum TipoWorkspace {
  Pessoal = 1,
  Agencia = 2,
  Empresa = 3,
}

export interface WorkspaceRegistroRequest {
  nome: string;
  tipo: TipoWorkspace;
}

export class RegistrarRequest {
  constructor(
    public codigo: string,
    public nome: string,
    public sobrenome: string,
    public email: string,
    public senha: string,
    public confirmaSenha: string,
    public workspace: WorkspaceRegistroRequest,
    public dataNascimento?: Date
  ) { }
}
