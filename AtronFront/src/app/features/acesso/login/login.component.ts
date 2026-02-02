import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService } from './services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';
import { LoginRequest } from '../../../shared/models/request/login-request.model';
import { VisualizacaoService } from '../../../core/services/visualizacao-service';
import { SessaoInfoService } from '../../../shared/services/sessaoInfo.service';

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
    public router: Router,
    public visualizacaoService: VisualizacaoService
  ) { }

  ngOnInit(): void {
    var codigoDeUsuarioLogado = this.sessaoService.getUsuarioCodigoLocalStorage();
    if (codigoDeUsuarioLogado) {
     this.sessaoService.clearSessionInfo();
    }

    this.form = this.fb.group({
      codigo: ['', Validators.required],
      senha: ['', Validators.required],
      //lembrar: ['']
    });
  }

  togglePasswordVisibility() {
    this.passwordType = this.passwordType === 'password' ? 'text' : 'password';
  }

  async autenticar() {
    let loginPayload = new LoginRequest();
    loginPayload.codigoDoUsuario = this.form.value.codigo;
    loginPayload.senha = this.form.value.senha;
    //loginPayload.lembrar = this.form.value.lembrar;

    this.loginService.autenticar(loginPayload).subscribe({
      next: () => {
        const rota = this.getRotaPorVisualizacao();
        this.router.navigate([rota]);
      },
      error: (error: any) => {
        console.log(error);
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
}