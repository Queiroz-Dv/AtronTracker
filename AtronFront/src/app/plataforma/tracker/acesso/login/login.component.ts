import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService } from '../../../../core/services/acesso.service';
import { NotificacaoService } from '../../../../core/services/notification.service';
import { SessaoInfoService } from '../../../../core/services/sessaoInfo.service';
import { VisualizacaoService } from '../../../../core/services/visualizacao-service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { LoginRequest } from '../../../../shared/models/request/login-request.model';
import { SharedModule } from '../../../../shared/modules/shared.module';


@Component({
  standalone: true,
  selector: 'c-login',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

export class LoginComponent implements OnInit {
  form!: FormGroup;
  id?: number;
  passwordType: string = 'password';
  constructor(
    private fb: FormBuilder,
    private loginService: AcessoService,
    private sessaoService: SessaoInfoService,
    private notificacaoService: NotificacaoService,
    public router: Router,
    public visualizacaoService: VisualizacaoService
  ) { }

  ngOnInit(): void {
    var codigoDeUsuarioLogado = this.sessaoService.obterUsuarioCodigo();
    if (codigoDeUsuarioLogado) {
     this.sessaoService.clearSessionInfo();
    }

    this.form = this.fb.group({
      codigo: ['', Validators.required],
      senha: ['', Validators.required]
    });
  }

  togglePasswordVisibility() {
    this.passwordType = this.passwordType === 'password' ? 'text' : 'password';
  }

  async autenticar() {
    let loginPayload = new LoginRequest();
    loginPayload.codigoDoUsuario = this.form.value.codigo;
    loginPayload.senha = this.form.value.senha;
    this.loginService.autenticar(loginPayload).subscribe({
      next: () => {
        const rota = this.getRotaPorVisualizacao();
        this.router.navigate([rota]);
      },
      error: (erro: any) => {
        const mensagens = this.notificacaoService.normalizarMensagens(erro?.error);
        if (mensagens.length) {
          this.notificacaoService.exibirMensagens(mensagens);
          return;
        }

        this.notificacaoService.exibirMensagem('Não foi possível realizar o login.', undefined, 6000);
      }
    });
  }

  private getRotaPorVisualizacao(): string {
    const modo = this.visualizacaoService.getViewMode();
    this.visualizacaoService.setViewMode(modo);
    return modo === 'menu' ? '/atron/home' : '/atron/dashboard';
  }

  registrar() {
    this.router.navigate(['/registrar']);
  }

  esqueciSenha() {
    this.router.navigate(['/esqueci-senha']);
  }

  reenviarConfirmacao() {
    this.router.navigate(['/reenviar-confirmacao']);
  }
}
