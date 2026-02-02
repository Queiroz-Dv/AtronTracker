import { ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';

export function senhasIguaisValidator(): ValidatorFn {
  return (group: AbstractControl): ValidationErrors | null => {
    const senha = group.get('senha')?.value;
    const confirmar = group.get('confirmarSenha')?.value;

    return senha === confirmar ? null : { senhasDiferentes: true };
  };
}
