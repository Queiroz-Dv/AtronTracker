export class RegistrarRequest {
  constructor(
    public codigo: string,
    public nome: string,
    public sobrenome: string,
    public email: string,
    public senha: string,
    public confirmaSenha: string,
    public dataNascimento?: Date,
    public codigoPerfilDeAcesso?: string,
    public clientUri?: string
  ) { }
}