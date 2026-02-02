import { ModuloModel } from "../../../modulos/interfaces/modulo.interface";

export class DadosDoUsuario {
  public nomeDoUsuario: string;
  public codigoDoUsuario: string;
  public email: string;
  public codigoDoDepartamento: string;
  public codigoDoCargo: string;

  public perfisDeAcesso: PerfilComModulos[];
}

export class PerfilComModulos
{
  public codigoPerfil: string;
  public modulos: ModuloModel[];
}