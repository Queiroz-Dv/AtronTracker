import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { UsuarioService } from '../../services/usuario.service';
import { Nivel, NotificacaoService } from '../../../../core/services/notification.service';

@Component({
  selector: 'c-usuario-configuracoes',
  imports: [ReactiveFormsModule, SharedModule],
  templateUrl: './usuario-configuracoes.component.html',
})
export class UsuarioConfiguracoesComponent implements OnInit {
  form!: FormGroup;
  carregando = false;

  private notificacaoService = inject(NotificacaoService);

  constructor(
    private fb: FormBuilder,
    private service: UsuarioService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      receberNotificacaoTarefaPorEmail: [false]
    });

    this.carregar();
  }

  carregar(): void {
    this.carregando = true;

    this.service.obterConfiguracoes().subscribe({
      next: configuracoes => {
        this.form.patchValue(configuracoes);
        this.carregando = false;
      },
      error: erro => {
        this.carregando = false;
        this.exibirErro(erro, 'Erro ao carregar configurações.');
      }
    });
  }

  salvar(): void {
    const configuracoes = this.form.getRawValue();

    this.service.atualizarConfiguracoes(configuracoes).subscribe({
      next: mensagens => this.notificacaoService.exibirMensagens(mensagens),
      error: erro => this.exibirErro(erro, 'Erro ao atualizar configurações.')
    });
  }

  private exibirErro(erro: unknown, mensagemPadrao: string): void {
    if (erro instanceof HttpErrorResponse) {
      this.notificacaoService.exibirMensagem(`Erro ${erro.status}: ${erro.statusText}`, Nivel.Error, 6000);
      return;
    }

    this.notificacaoService.exibirMensagem(mensagemPadrao, Nivel.Error, 6000);
  }
}
