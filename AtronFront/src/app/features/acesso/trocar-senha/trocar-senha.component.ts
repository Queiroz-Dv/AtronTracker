import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AcessoService, RedefinirSenhaRequest } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';
import { CryptoService } from '../../../core/services/crypto/crypto.service';

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
  erroLink = false;

  private identificadorTemporario = '';

  private readonly SESSION_KEY = 'atron_id_temp';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private acessoService: AcessoService,
    private cryptoService: CryptoService
  ) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const idCriptografado = params['id'] || '';

      if (idCriptografado) {
        // Descriptografar o identificador temporário que veio na URL
        const idDescriptografado = this.cryptoService.decrypt(idCriptografado);

        if (idDescriptografado) {
          this.identificadorTemporario = idDescriptografado;
          // Guardar no sessionStorage para resiliência em caso de refresh
          sessionStorage.setItem(this.SESSION_KEY, this.identificadorTemporario);
        } else {
          // Falha na descriptografia — link inválido ou adulterado
          this.erroLink = true;
        }
      } else {
        // Tentar recuperar do sessionStorage (caso de refresh da página)
        const idSalvo = sessionStorage.getItem(this.SESSION_KEY);
        if (idSalvo) {
          this.identificadorTemporario = idSalvo;
        } else {
          this.erroLink = true;
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

    const pw1 = this.form.get('senha')?.value;
    const pw2 = this.form.get('repetirSenha')?.value;

    if (pw1 !== pw2) {
      alert('As senhas não coincidem!');
      return;
    }

    let req = new RedefinirSenhaRequest();
    req.identificadorTemporario = this.identificadorTemporario;

    // Criptografar senhas na requisição
    req.novaSenha = this.cryptoService.encrypt(pw1);
    req.repetirSenha = this.cryptoService.encrypt(pw2);

    this.acessoService.trocarSenha(req).subscribe({
      next: (res: any) => {
        this.senhaSalva = true;
        this.mensagemEnvio = "Sua senha foi redefinida com sucesso.";
        // Limpar sessionStorage após sucesso
        sessionStorage.removeItem(this.SESSION_KEY);
      },
      error: (err: any) => {
        console.error(err);
        alert('Ocorreu um erro ao redefinir a senha. Verifique se o link expirou.');
      }
    });
  }

  voltarLogin() {
    sessionStorage.removeItem(this.SESSION_KEY);
    this.router.navigate(['/login']);
  }
}
