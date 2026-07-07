export interface NotificacaoInterna {
  id: number;
  titulo: string;
  mensagem: string;
  modulo: string;
  tipoEvento: string;
  urlDestino: string;
  tarefaId?: number | null;
  lida: boolean;
  dataCriacao: string;
  dataLeitura?: string | null;
}
