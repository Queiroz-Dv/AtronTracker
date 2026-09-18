import { Mensagem } from '../../../core/services/notification.service';

export interface WorkspaceResponse {  
  codigo: string; 
}

export interface RegistrarResponse {
  usuarioCodigo: string;
  workspace: WorkspaceResponse;
  mensagens: Mensagem[];
}
