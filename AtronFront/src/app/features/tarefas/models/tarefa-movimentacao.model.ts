export interface TarefaMovimentacao {
  id: number;
  movimento: string;
  detalhes: string;
  responsavelCodigo: string;
  responsavelNome: string;
  dataOcorrencia: string;
}

export interface TarefaMovimentacaoPagina {
  itens: TarefaMovimentacao[];
  totalItens: number;
  pagina: number;
  tamanhoPagina: number;
}
