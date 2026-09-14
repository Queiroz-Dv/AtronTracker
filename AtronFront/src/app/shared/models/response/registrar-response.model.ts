import { Mensagem } from '../../../core/services/notification.service';

export interface WorkspaceResponse {
  id: number;
  nome: string;
  empresaCodigo: string | null;
}

export interface WorkspaceInicialResponse extends WorkspaceResponse { }

export interface RegistrarResponse {
  usuarioCodigo: string;
  workspace: WorkspaceInicialResponse;
  mensagens: Mensagem[];
}
