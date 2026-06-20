import { Component } from '@angular/core';
import { FormControl, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService, SolicitarRecuperacaoSenhaRequest } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';

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

    let req = new SolicitarRecuperacaoSenhaRequest();
    req.identificador = this.identificadorControl.value as string;
    // Pega o origin para criar a base url que vai no link do e-mail
    req.clientUri = window.location.origin;

    this.acessoService.solicitarRecuperacaoSenha(req).subscribe({
      next: (res: any) => {
        this.enviado = true;
        this.mensagemEnvio = "Se o código/e-mail estiver correto, enviaremos as instruções para seu e-mail.";
      },
      error: (err: any) => {
        // Exibir alerta?
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
