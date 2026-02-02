import { Component, Input } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { FormErrorMessages } from './form-error-messages';
import { SharedModule } from '../../modules/shared.module';

@Component({
  selector: 'c-control-error',
  imports: [SharedModule],
  templateUrl: './control-error.component.html',
})
export class ControlErrorComponent {
  @Input() control!: AbstractControl | null;

  get errorKeys(): string[] {
    if (!this.control?.errors) return [];
    return Object.keys(this.control.errors);
  }

  getErrorMessage(errorKey: string): string {
    const error = this.control?.getError(errorKey);
    return FormErrorMessages.getMessage(errorKey, error)
  }
}