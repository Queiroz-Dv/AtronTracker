import { RouterModule, Routes } from "@angular/router";
import { SalarioEditComponent } from "./components/salario-edit/salario-edit.component";
import { SalarioViewComponent } from "./components/salario-view/salario-view.component";
import { NgModule } from "@angular/core";

const SALARIO_ROUTES: Routes = [
  { path: '', component: SalarioViewComponent },
  { path: 'novo', component: SalarioEditComponent },
  { path: 'editar/:id', component: SalarioEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(SALARIO_ROUTES)],
  exports: [RouterModule],
})

export class SalarioRoutingModule { }