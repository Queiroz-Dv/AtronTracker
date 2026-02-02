import { RouterModule, Routes } from "@angular/router";
import { CargoEditComponent } from "./components/cargo-edit/cargo-edit.component";
import { CargosViewComponent } from "./components/cargo-view/cargo-view.component";
import { NgModule } from "@angular/core";

const CARGO_ROUTES: Routes = [
  { path: '', component: CargosViewComponent },
  { path: 'novo', component: CargoEditComponent },
  { path: 'editar/:codigo', component: CargoEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(CARGO_ROUTES)],
  exports: [RouterModule],
})

export class CargoRoutingModule { } 