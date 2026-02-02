import { PerfilDeAcessoModel } from "../../perfil-de-acesso/interfaces/perfil-de-acesso.interface";

export class PerfilUsuarioModel {
  perfilDeAcesso: PerfilDeAcessoModel
  usuarios: UsuarioPerfilDTO[]
}

export class UsuarioPerfilDTO {
  codigo: string;
  nome: string;
  sobrenome: string;
}