import { RouterModule, Routes } from "@angular/router";
import { TarefaEditComponent } from "./components/tarefa-edit/tarefa-edit.component";
import { TarefaViewComponent } from "./components/tarefa-view/tarefa-view.component";
import { NgModule } from "@angular/core";

const TAREFA_ROUTES: Routes = [
  { path: '', component: TarefaViewComponent },
  { path: 'novo', component: TarefaEditComponent },
  { path: 'editar/:id', component: TarefaEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(TAREFA_ROUTES)],
  exports: [RouterModule],
})

export class TarefaRoutingModule { }
