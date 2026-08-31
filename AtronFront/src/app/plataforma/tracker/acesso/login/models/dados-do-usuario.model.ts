import { ModuloModel } from "../../../../../features/navegacao/modulos/interfaces/modulo.interface";
import { WorkspaceResponse } from "../../../../../shared/models/response/registrar-response.model";

export class DadosDoUsuario {
  public nomeDoUsuario: string;
  public codigoDoUsuario: string;
  public email: string;
  public codigoDoDepartamento: string;
  public codigoDoCargo: string;

  public perfisDeAcesso: PerfilComModulos[];
  public workspaceAtual: WorkspaceResponse | null;
}

export class PerfilComModulos
{
  public codigoPerfil: string;
  public modulos: ModuloModel[];
}
