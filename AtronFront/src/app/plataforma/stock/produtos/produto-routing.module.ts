import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProdutoEditComponent } from './components/produto-edit/produto-edit.component';
import { ProdutoViewComponent } from './components/produto-view/produto-view.component';
import { ProcessamentoProdutoDetalheComponent } from './processamentos/components/processamento-produto-detalhe/processamento-produto-detalhe.component';
import { ProcessamentoProdutoListaComponent } from './processamentos/components/processamento-produto-lista/processamento-produto-lista.component';

export const PRODUTO_ROUTES: Routes = [
  { path: '', component: ProdutoViewComponent },
  { path: 'novo', component: ProdutoEditComponent },
  { path: 'editar/:codigo', component: ProdutoEditComponent },
  { path: 'processamentos', component: ProcessamentoProdutoListaComponent },
  { path: 'processamentos/:id', component: ProcessamentoProdutoDetalheComponent }
];

@NgModule({
  imports: [RouterModule.forChild(PRODUTO_ROUTES)],
  exports: [RouterModule]
})
export class ProdutoRoutingModule { }
