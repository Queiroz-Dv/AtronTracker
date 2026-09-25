export class WorkspaceRegistroRequest {
  constructor(public codigo: string, public descricao: string) {
  }
}
export class RegistrarRequest {
  workspace: WorkspaceRegistroRequest;
  constructor(
    public codigo: string,
    public nome: string,
    public sobrenome: string,
    public email: string,
    public senha: string,
    public confirmaSenha: string,
    public dataNascimento?: string,
    public convite?: string
  ) { }
}