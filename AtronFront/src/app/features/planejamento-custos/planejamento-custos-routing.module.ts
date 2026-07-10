import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlanejamentoCustosViewComponent } from './components/planejamento-custos-view/planejamento-custos-view.component';
import { PlanejamentoCustosRelatorioComponent } from './components/planejamento-custos-relatorio/planejamento-custos-relatorio.component';
import { PlanejamentoCustosEditComponent } from './components/planejamento-custos-edit/planejamento-custos-edit.component';

const PLANEJAMENTO_CUSTOS_ROUTES: Routes = [
  { path: '', component: PlanejamentoCustosViewComponent },
  { path: 'novo', component: PlanejamentoCustosEditComponent },
  { path: 'editar/:codigo', component: PlanejamentoCustosEditComponent },
  { path: 'relatorio-geral', component: PlanejamentoCustosRelatorioComponent },
  { path: 'relatorio/:codigo', component: PlanejamentoCustosRelatorioComponent },
];

@NgModule({
  imports: [RouterModule.forChild(PLANEJAMENTO_CUSTOS_ROUTES)],
  exports: [RouterModule],
})
export class PlanejamentoCustosRoutingModule { }
