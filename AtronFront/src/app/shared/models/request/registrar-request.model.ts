export enum TipoWorkspace {
  Pessoal = 1,
  Empresa = 3,
}

export interface WorkspaceRegistroRequest {
  nome: string;
  tipo: TipoWorkspace;
  empresa?: EmpresaRegistroRequest;
}

export interface EmpresaRegistroRequest {
  codigo: string;
  nomeFantasia: string;
  endereco: string;
  numero: string;
  email: string;
}

export class RegistrarRequest {
  constructor(
    public codigo: string,
    public nome: string,
    public sobrenome: string,
    public email: string,
    public senha: string,
    public confirmaSenha: string,
    public workspace?: WorkspaceRegistroRequest,
    public dataNascimento?: string,
    public convite?: string
  ) { }
}
