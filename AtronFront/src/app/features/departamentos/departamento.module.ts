import { NgModule } from '@angular/core';
import { DepartamentoRoutingModule } from './departamento-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { DepartamentoEditComponent } from './components/departamento-edit/departamento-edit.component';
import { DepartamentoViewComponent } from './components/departamento-view/departamento-view.component';
import { SharedModule } from '../../shared/modules/shared.module';

@NgModule({
  declarations: [
  ],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    DepartamentoViewComponent,
    DepartamentoEditComponent,
    DepartamentoRoutingModule
  ]
})
export class DepartamentoModule { }
