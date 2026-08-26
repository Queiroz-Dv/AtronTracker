export enum CategoriaStatus {
  Ativo = 1,
  Inativo = 2,
  Removido = 3
}

export interface Categoria {
  codigo: string;
  descricao: string;
  status: CategoriaStatus;
}

export interface CategoriaHistoricoAuditoria {
  codigoHistorico: number;
  dataCriacao: string;
  descricao: string;
}

export interface CategoriaAuditoria {
  codigoRegistro: string;
  removidoEm: string | null;
  historicos: CategoriaHistoricoAuditoria[];
}

export function obterDescricaoCategoriaStatus(status: CategoriaStatus): string {
  switch (status) {
    case CategoriaStatus.Ativo:
      return 'Ativa';
    case CategoriaStatus.Inativo:
      return 'Inativa';
    case CategoriaStatus.Removido:
      return 'Removida';
    default:
      return 'Desconhecido';
  }
}
