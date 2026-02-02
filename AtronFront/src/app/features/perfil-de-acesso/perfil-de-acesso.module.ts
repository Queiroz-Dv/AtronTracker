import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../../shared/modules/shared.module";
import { PerfilDeAcessoEditComponent } from "./components/perfil-de-acesso-edit/perfil-de-acesso-edit.component";
import { PerfilDeAcessoViewComponent } from "./components/perfil-de-acesso-view/perfil-de-acesso-view.component";
import { PerfilRoutingModule } from "./perfil-de-acesso-routing.module";


@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    PerfilDeAcessoViewComponent,
    PerfilDeAcessoEditComponent,
    PerfilRoutingModule
  ]
})

export class PerfilModule { }