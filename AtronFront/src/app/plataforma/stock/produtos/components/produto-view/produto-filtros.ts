import { Produto, ProdutoStatus } from '../../models/produto.model';

export interface ProdutoFiltros {
  texto: string;
  status: ProdutoStatus | null;
  lote: string;
}

export const FILTROS_PRODUTO_INICIAIS: ProdutoFiltros = {
  texto: '',
  status: null,
  lote: ''
};

export function produtoCorrespondeAosFiltros(
  produto: Produto,
  filtroSerializado: string
): boolean {
  const filtros = JSON.parse(filtroSerializado) as ProdutoFiltros;
  const textoProduto = `${produto.codigo} ${produto.descricao}`.toLowerCase();
  const possuiTexto = textoProduto.includes(filtros.texto);
  const possuiStatus = filtros.status === null || produto.status === filtros.status;
  const pertenceAoLote = !filtros.lote || produto.loteProdutoCodigo === filtros.lote;
  return possuiTexto && possuiStatus && pertenceAoLote;
}
