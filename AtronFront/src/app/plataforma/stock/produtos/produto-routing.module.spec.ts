import { ProdutoEditComponent } from './components/produto-edit/produto-edit.component';
import { ProdutoViewComponent } from './components/produto-view/produto-view.component';
import { ProcessamentoProdutoDetalheComponent } from './processamentos/components/processamento-produto-detalhe/processamento-produto-detalhe.component';
import { ProcessamentoProdutoListaComponent } from './processamentos/components/processamento-produto-lista/processamento-produto-lista.component';
import { PRODUTO_ROUTES } from './produto-routing.module';

describe('Rotas de Produto', () => {
  it('mantém o cadastro e adiciona o acompanhamento de processamentos', () => {
    expect(PRODUTO_ROUTES.map(rota => [rota.path, rota.component])).toEqual([
      ['', ProdutoViewComponent],
      ['novo', ProdutoEditComponent],
      ['editar/:codigo', ProdutoEditComponent],
      ['processamentos', ProcessamentoProdutoListaComponent],
      ['processamentos/:id', ProcessamentoProdutoDetalheComponent]
    ]);
  });
});
