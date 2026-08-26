import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ControlErrorComponent } from '../../../../../shared/components/control-error/control-error.component';
import { SharedModule } from '../../../../../shared/modules/shared.module';

@Component({
  selector: 'c-categoria-form',
  standalone: true,
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './categoria-form.component.html'
})
export class CategoriaFormComponent {
  @Input({ required: true }) form!: FormGroup;
  @Input() editando = false;
}
