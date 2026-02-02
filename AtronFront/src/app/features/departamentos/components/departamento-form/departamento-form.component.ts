import { Component, Input, } from '@angular/core';
import { FormGroup, ReactiveFormsModule, } from '@angular/forms';
import { ControlErrorComponent } from "../../../../shared/components/control-error/control-error.component";
import { SharedModule } from '../../../../shared/modules/shared.module';

@Component({
  standalone: true,
  selector: 'c-departamento-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './departamento-form.component.html',
})

export class DepartamentoFormComponent {
  @Input() form!: FormGroup;
}