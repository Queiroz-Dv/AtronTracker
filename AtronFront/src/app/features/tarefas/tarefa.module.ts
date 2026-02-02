import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../../shared/modules/shared.module";
import { TarefaEditComponent } from "./components/tarefa-edit/tarefa-edit.component";
import { TarefaViewComponent } from "./components/tarefa-view/tarefa-view.component";
import { TarefaRoutingModule } from "./tarefa-routing.module";

@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    TarefaEditComponent,
    TarefaViewComponent,
    TarefaRoutingModule
  ]
})

export class TarefaModule { }