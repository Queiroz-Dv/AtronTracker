export enum ProdutoStatus {
  Ativo = 1,
  Baixado = 2
}

export interface CategoriaProduto {
  codigo: string;
  descricao: string;
}

export interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  descricaoComplementar: string | null;
  dataAquisicao: string | null;
  precoUnitario: number | null;
  dataEfetivaBaixa: string | null;
  status: ProdutoStatus;
  loteProdutoId: number | null;
  loteProdutoCodigo: string | null;
  categorias: CategoriaProduto[];
}

export interface ProdutoCriacao {
  codigo: string;
  descricao: string;
  descricaoComplementar: string | null;
  dataAquisicao: string;
  precoUnitario: number;
  categoriaCodigos: string[];
}

export interface ProdutoFormulario extends ProdutoCriacao {
  gerarPorLote: boolean;
  quantidade: number | null;
}

export interface GeracaoProdutosLoteRequest {
  codigoBase: string;
  quantidade: number;
  descricao: string;
  descricaoComplementar: string | null;
  dataAquisicao: string;
  precoUnitario: number;
  categoriaCodigos: string[];
}

export interface SolicitacaoGeracaoProdutosLoteResponse {
  processamentoId: number;
  status: number;
}

export type ProdutoAtualizacao = Omit<ProdutoCriacao, 'codigo'>;

export interface ProdutoHistoricoAuditoria {
  codigoHistorico: number;
  dataCriacao: string;
  descricao: string;
}

export interface ProdutoAuditoria {
  codigoRegistro: string;
  removidoEm: string | null;
  historicos: ProdutoHistoricoAuditoria[];
}

export function obterDescricaoProdutoStatus(status: ProdutoStatus): string {
  return status === ProdutoStatus.Ativo ? 'Ativo' : 'Baixado';
}
