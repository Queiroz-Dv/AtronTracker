import { Produto, ProdutoStatus } from '../../models/produto.model';
import { ProdutoFiltros, produtoCorrespondeAosFiltros } from './produto-filtros';

describe('produtoCorrespondeAosFiltros', () => {
  it('deve restringir os produtos ao lote informado pelo processamento', () => {
    const filtros: ProdutoFiltros = { texto: '', status: null, lote: 'LOT-10' };

    expect(produtoCorrespondeAosFiltros(criarProduto('LOT-10'), JSON.stringify(filtros))).toBeTrue();
    expect(produtoCorrespondeAosFiltros(criarProduto('LOT-20'), JSON.stringify(filtros))).toBeFalse();
  });
});

function criarProduto(loteProdutoCodigo: string): Produto {
  return {
    id: 1, codigo: 'NOTE001', descricao: 'Notebook', descricaoComplementar: null,
    dataAquisicao: '2026-08-24', precoUnitario: 1000, status: ProdutoStatus.Ativo,
    dataEfetivaBaixa: null, loteProdutoId: 1, loteProdutoCodigo, categorias: []
  };
}
