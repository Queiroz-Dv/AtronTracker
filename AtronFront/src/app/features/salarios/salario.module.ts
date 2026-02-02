import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../../shared/modules/shared.module";
import { SalarioEditComponent } from "./components/salario-edit/salario-edit.component";
import { SalarioViewComponent } from "./components/salario-view/salario-view.component";
import { SalarioRoutingModule } from "./salario-routing.module";

@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    SalarioEditComponent,
    SalarioViewComponent,
    SalarioRoutingModule
  ]
})

export class SalarioModule { }