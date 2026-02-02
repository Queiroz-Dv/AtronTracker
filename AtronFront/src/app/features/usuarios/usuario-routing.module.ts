import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { UsuarioEditComponent } from "./components/usuario-edit/usuario-edit.component";
import { UsuarioViewComponent } from "./components/usuario-view/usuario-view.component";

// Declaração das rotas do módulo de usuários
const USUARIO_ROUTES: Routes = [
  { path: '', component: UsuarioViewComponent },
  { path: 'novo', component: UsuarioEditComponent },
  { path: 'editar/:codigo', component: UsuarioEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(USUARIO_ROUTES)],
  exports: [RouterModule],
})

export class UsuarioRoutingModule { }