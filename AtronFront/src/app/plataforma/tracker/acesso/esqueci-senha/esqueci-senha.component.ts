import { Component } from '@angular/core';
import { FormControl, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AcessoService, SolicitarRecuperacaoSenhaRequest } from '../../../../core/services/acesso.service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { SharedModule } from '../../../../shared/modules/shared.module';


@Component({
  standalone: true,
  selector: 'c-esqueci-senha',
  templateUrl: './esqueci-senha.component.html',
  styleUrls: ['../login/login.component.css'],
  imports: [SharedModule, ReactiveFormsModule, FormsModule, ControlErrorComponent, RouterModule]
})
export class EsqueciSenhaComponent {
  identificadorControl = new FormControl('', [Validators.required]);
  enviado = false;
  mensagemEnvio = '';

  constructor(
    private router: Router,
    private acessoService: AcessoService
  ) { }

  async solicitarRecuperacao() {
    if (this.identificadorControl.invalid) {
      return;
    }

    const req = new SolicitarRecuperacaoSenhaRequest();
    req.identificador = this.identificadorControl.value as string;
    req.clientUri = window.location.origin;

    this.acessoService.solicitarRecuperacaoSenha(req).subscribe({
      next: () => {
        this.enviado = true;
        this.mensagemEnvio = "Se o código/e-mail estiver correto, enviaremos as instruções para seu e-mail.";
      },
      error: (err: any) => {
        console.error(err);
        this.enviado = true;
        this.mensagemEnvio = "Houve um erro, verifique se o usuário consta em base e não está inativo.";
      }
    });
  }

  voltarLogin() {
    this.router.navigate(['/login']);
  }

  registrar() {
    this.router.navigate(['/registrar']);
  }
}
