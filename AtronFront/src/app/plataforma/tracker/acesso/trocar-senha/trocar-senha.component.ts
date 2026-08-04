import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AcessoService, RedefinirSenhaRequest } from '../../../../core/services/acesso.service';
import { NotificacaoService, Nivel } from '../../../../core/services/notification.service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { SharedModule } from '../../../../shared/modules/shared.module';

@Component({
  standalone: true,
  selector: 'app-trocar-senha',
  templateUrl: './trocar-senha.component.html',
  styleUrls: ['../login/login.component.css'],
  imports: [SharedModule, ReactiveFormsModule, FormsModule, ControlErrorComponent, RouterModule]
})
export class TrocarSenhaComponent implements OnInit {
  form!: FormGroup;
  passwordType: string = 'password';
  repeatPasswordType: string = 'password';
  senhaSalva = false;
  mensagemEnvio = '';
  mensagemErro = '';
  erroLink = false;

  private identificadorTemporario = '';
  private readonly SESSION_KEY = 'atron_id_temp';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private acessoService: AcessoService,
    private notificacaoService: NotificacaoService
  ) { }

  ngOnInit(): void {
    this.route.fragment.subscribe(fragmento => {
      const token = new URLSearchParams(fragmento ?? '').get('token') ?? '';

      if (token) {
        this.identificadorTemporario = token;
        sessionStorage.setItem(this.SESSION_KEY, token);
        window.history.replaceState(null, '', `${window.location.pathname}${window.location.search}`);
      } else {
        const tokenSalvo = sessionStorage.getItem(this.SESSION_KEY);
        if (tokenSalvo) {
          this.identificadorTemporario = tokenSalvo;
        } else {
          this.marcarLinkInvalido();
        }
      }
    });

    this.form = this.fb.group({
      senha: ['', Validators.required],
      repetirSenha: ['', [Validators.required]]
    });
  }

  togglePasswordVisibility() {
    this.passwordType = this.passwordType === 'password' ? 'text' : 'password';
  }

  toggleRepeatPasswordVisibility() {
    this.repeatPasswordType = this.repeatPasswordType === 'password' ? 'text' : 'password';
  }

  salvarNovaSenha() {
    if (this.form.invalid) return;
    this.mensagemErro = '';

    const pw1 = this.form.get('senha')?.value;
    const pw2 = this.form.get('repetirSenha')?.value;

    if (pw1 !== pw2) {
      this.exibirErro('As senhas nao coincidem.');
      return;
    }

    const req = new RedefinirSenhaRequest();
    req.identificadorTemporario = this.identificadorTemporario;
    req.novaSenha = pw1;
    req.repetirSenha = pw2;

    this.acessoService.trocarSenha(req).subscribe({
      next: () => {
        this.senhaSalva = true;
        this.mensagemEnvio = 'Sua senha foi redefinida com sucesso.';
        this.notificacaoService.exibirMensagem(this.mensagemEnvio, Nivel.Sucesso);
        sessionStorage.removeItem(this.SESSION_KEY);
      },
      error: (err: any) => {
        console.error(err);
        const mensagem = this.extrairMensagemErro(err)
          || 'Ocorreu um erro ao redefinir a senha. Verifique se o link expirou.';
        this.exibirErro(mensagem);
      }
    });
  }

  voltarLogin() {
    sessionStorage.removeItem(this.SESSION_KEY);
    this.router.navigate(['/login']);
  }

  private marcarLinkInvalido(): void {
    this.erroLink = true;
    this.exibirErro('Link invalido ou expirado. Solicite um novo link para redefinir a senha.');
  }

  private exibirErro(mensagem: string): void {
    this.mensagemErro = mensagem;
    this.notificacaoService.exibirMensagem(mensagem, Nivel.Error, 7000);
  }

  private extrairMensagemErro(erro: any): string {
    const mensagens = this.notificacaoService.normalizarMensagens(erro?.error);
    return mensagens[0]?.descricao
      || (Array.isArray(erro?.error) ? erro.error.join(' ') : '')
      || (typeof erro?.error === 'string' ? erro.error : '')
      || erro?.message
      || '';
  }
}
