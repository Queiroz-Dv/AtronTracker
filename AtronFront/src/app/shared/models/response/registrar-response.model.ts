import { Mensagem } from '../../../core/services/notification.service';
import { TipoWorkspace } from '../request/registrar-request.model';

export interface WorkspaceResponse {
  id: number;
  nome: string;
  tipo: TipoWorkspace;
  empresaCodigo: string | null;
}

export interface WorkspaceInicialResponse extends WorkspaceResponse { }

export interface RegistrarResponse {
  usuarioCodigo: string;
  workspace: WorkspaceInicialResponse;
  mensagens: Mensagem[];
}

export interface ConviteWorkspaceResponse {
  workspace: WorkspaceInicialResponse;
  remetenteCodigo: string;
  expiraEm: string;
}
