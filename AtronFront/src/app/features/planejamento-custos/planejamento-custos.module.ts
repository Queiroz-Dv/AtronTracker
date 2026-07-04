import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/modules/shared.module';
import { PlanejamentoCustosRoutingModule } from './planejamento-custos-routing.module';
import { PlanejamentoCustosViewComponent } from './components/planejamento-custos-view/planejamento-custos-view.component';
import { PlanejamentoCustosRelatorioComponent } from './components/planejamento-custos-relatorio/planejamento-custos-relatorio.component';
import { PlanejamentoCustosEditComponent } from './components/planejamento-custos-edit/planejamento-custos-edit.component';

@NgModule({
  declarations: [],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    PlanejamentoCustosViewComponent,
    PlanejamentoCustosEditComponent,
    PlanejamentoCustosRelatorioComponent,
    PlanejamentoCustosRoutingModule
  ]
})
export class PlanejamentoCustosModule { }
