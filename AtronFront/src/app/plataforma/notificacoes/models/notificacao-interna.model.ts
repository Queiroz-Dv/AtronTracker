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

export function normalizarTextoNotificacao(texto: string): string {
  return texto
    .replace(/\batribuida\b/g, 'atribuída')
    .replace(/\bvoce\b/g, 'você')
    .replace(/\bNotificacao\b/g, 'Notificação')
    .replace(/\bNotificacoes\b/g, 'Notificações')
    .replace(/\bnotificacao\b/g, 'notificação')
    .replace(/\bnotificacoes\b/g, 'notificações');
}
