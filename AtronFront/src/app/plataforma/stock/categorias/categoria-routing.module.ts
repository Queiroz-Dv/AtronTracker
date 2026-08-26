import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoriaEditComponent } from './components/categoria-edit/categoria-edit.component';
import { CategoriaViewComponent } from './components/categoria-view/categoria-view.component';

const CATEGORIA_ROUTES: Routes = [
  { path: '', component: CategoriaViewComponent },
  { path: 'novo', component: CategoriaEditComponent },
  { path: 'editar/:codigo', component: CategoriaEditComponent }
];

@NgModule({
  imports: [RouterModule.forChild(CATEGORIA_ROUTES)],
  exports: [RouterModule]
})
export class CategoriaRoutingModule { }
