import { Component } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService, ReenviarConfirmacaoEmailRequest } from '../../../../core/services/acesso.service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { SharedModule } from '../../../../shared/modules/shared.module';

@Component({
  standalone: true,
  selector: 'c-reenviar-confirmacao',
  templateUrl: './reenviar-confirmacao.component.html',
  styleUrls: ['../login/login.component.css'],
  imports: [SharedModule, ReactiveFormsModule, FormsModule, ControlErrorComponent, RouterModule]
})
export class ReenviarConfirmacaoComponent {
  identificadorControl = new FormControl('', [Validators.required]);
  enviado = false;
  mensagemEnvio = '';

  constructor(
    private router: Router,
    private acessoService: AcessoService
  ) { }

  solicitarReenvio() {
    if (this.identificadorControl.invalid) {
      this.identificadorControl.markAsTouched();
      return;
    }

    const request = new ReenviarConfirmacaoEmailRequest();
    request.identificador = this.identificadorControl.value as string;
    this.acessoService.reenviarConfirmacaoEmail(request).subscribe({
      next: () => {
        this.enviado = true;
        this.mensagemEnvio = 'Se os dados estiverem corretos, enviaremos um novo link de confirmação para o e-mail cadastrado.';
      },
      error: () => {
        this.enviado = true;
        this.mensagemEnvio = 'Não foi possível reenviar o link. Verifique se o usuário existe, está ativo e ainda não confirmou o e-mail.';
      }
    });
  }

  voltarLogin() {
    this.router.navigate(['/login']);
  }

  esqueciSenha() {
    this.router.navigate(['/esqueci-senha']);
  }
}
