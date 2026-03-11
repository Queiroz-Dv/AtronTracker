import { RouterModule, Routes } from '@angular/router';
import { DepartamentoViewComponent } from './components/departamento-view/departamento-view.component';
import { DepartamentoEditComponent } from './components/departamento-edit/departamento-edit.component';
import { NgModule } from '@angular/core';

const DEPARTAMENTO_ROUTES: Routes = [
  { path: '', component: DepartamentoViewComponent },
  { path: 'novo', component: DepartamentoEditComponent },
  { path: 'editar/:codigo', component: DepartamentoEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(DEPARTAMENTO_ROUTES)],
  exports: [RouterModule],
})

export class DepartamentoRoutingModule { }