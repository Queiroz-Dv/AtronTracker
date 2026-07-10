import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AcessoService } from '../login/services/acesso.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../shared/components/control-error/control-error.component';
import { senhasIguaisValidator } from '../../../core/validators/senhasIguaisValidator.validator';
import { RegistrarRequest } from '../../../shared/models/request/registrar-request.model';
import { Nivel, NotificacaoService } from '../../../core/services/notification.service';

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

  constructor(
    private fb: FormBuilder,
    private acessoService: AcessoService,
    private notificacaoService: NotificacaoService,
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
      next: (resposta: unknown) => {
        const mensagens = this.notificacaoService.normalizarMensagens(resposta, Nivel.Sucesso);
        const mensagemSucesso = mensagens.length
          ? mensagens.map(mensagem => mensagem.descricao).join('\n')
          : 'Usuário registrado com sucesso! Verifique seu e-mail para confirmar.';

        this.notificacaoService.exibirMensagem(mensagemSucesso, Nivel.Sucesso, 5000);

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (error: any) => {
        console.error('Erro ao registrar usuário:', error);
        const mensagens = this.notificacaoService.normalizarMensagens(error);

        if (mensagens.length) {
          this.notificacaoService.exibirMensagens(mensagens);
          return;
        }

        this.notificacaoService.exibirMensagem('Erro ao registrar usuário. Tente novamente.', Nivel.Error);
      }
    });
  }
}
