import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';
import { senhasIguaisValidator } from '../../../core/validators/senhasIguaisValidator.validator';
import { RegistrarRequest } from '../../../shared/models/request/registrar-request.model';

@Component({
  selector: 'c-registrar',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, RouterModule],
  templateUrl: './registrar.component.html',
  styleUrls: ['./registrar.component.css'],
  standalone: true,
})

export class RegistrarComponent implements OnInit {
  form!: FormGroup;
  id?: number;
  mensagemSucesso = '';
  mensagemErro = '';

  constructor(
    private fb: FormBuilder,
    private acessoService: AcessoService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', Validators.required],
      nome: ['', Validators.required],
      sobrenome: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      dataNascimento: ['', Validators.required],
      codigoPerfilDeAcesso: [''],
      senha: ['', Validators.required],
      confirmaSenha: ['', Validators.required]
    }, { validators: [senhasIguaisValidator()] });
  }

  registrarNovoUsuario(): void {
    this.mensagemSucesso = '';
    this.mensagemErro = '';

    const clientUri = window.location.origin;

    const dadosDoUsuario = new RegistrarRequest(
      this.form.value.codigo,
      this.form.value.nome,
      this.form.value.sobrenome,
      this.form.value.email,
      this.form.value.senha,
      this.form.value.confirmaSenha,
      this.form.value.dataNascimento,
      this.form.value.codigoPerfilDeAcesso || undefined,
      clientUri || undefined);

    this.acessoService.registrar(dadosDoUsuario).subscribe({
      next: (mensagens: string[]) => {
        // Se recebeu resposta 200, o registro foi bem-sucedido
        this.mensagemSucesso = mensagens?.length > 0
          ? mensagens.join(' ')
          : 'Usuário registrado com sucesso! Verifique seu e-mail para confirmar.';
        console.log('Usuário registrado com sucesso!', mensagens);

        // Aguarda 3 segundos para o usuário ler a mensagem antes de redirecionar
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (error: any) => {
        console.error('Erro ao registrar usuário:', error);

        if (error?.error && Array.isArray(error.error)) {
          this.mensagemErro = error.error.join(' ');
        } else if (error?.error?.message) {
          this.mensagemErro = error.error.message;
        } else if (typeof error?.error === 'string') {
          this.mensagemErro = error.error;
        } else {
          this.mensagemErro = 'Erro ao registrar usuário. Tente novamente.';
        }
      }
    });
  }
}