import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/modules/shared.module';
import { CargosViewComponent } from './components/cargo-view/cargo-view.component';
import { CargoEditComponent } from './components/cargo-edit/cargo-edit.component';
import { CargoRoutingModule } from './cargo-routing.module';

@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    CargosViewComponent,
    CargoEditComponent,
    CargoRoutingModule
  ]
})
export class CargoModule { }
