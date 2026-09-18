import { ModuloModel } from "../../../../../features/navegacao/modulos/interfaces/modulo.interface";

export class DadosDoUsuario {  
  public codigoDoUsuario: string;
  public emailDoUsuario: string;
  public codigoDoDepartamento: string;
  public codigoDoCargo: string;
  public perfisDeAcesso: PerfilComModulos[];
  public workspace: string;
}

export class PerfilComModulos
{
  public codigoPerfil: string;
  public modulos: ModuloModel[];
}