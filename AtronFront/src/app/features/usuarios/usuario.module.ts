import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/modules/shared.module';
import { UsuarioEditComponent } from './components/usuario-edit/usuario-edit.component';
import { UsuarioViewComponent } from './components/usuario-view/usuario-view.component';
import { UsuarioRoutingModule } from './usuario-routing.module';

@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    UsuarioViewComponent,
    UsuarioEditComponent,
    UsuarioRoutingModule
  ]
})

export class UsuarioModule { }