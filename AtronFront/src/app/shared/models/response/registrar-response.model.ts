import { Mensagem } from '../../../core/services/notification.service';
import { TipoWorkspace } from '../request/registrar-request.model';

export interface WorkspaceInicialResponse {
  id: number;
  nome: string;
  tipo: TipoWorkspace;
  empresaCodigo: string | null;
}

export interface RegistrarResponse {
  usuarioCodigo: string;
  workspace: WorkspaceInicialResponse;
  mensagens: Mensagem[];
}
