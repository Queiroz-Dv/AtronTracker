import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { TarefaService } from '../../services/tarefa.service';

@Component({
  selector: 'c-tarefa-configuracoes',
  standalone: true,
  imports: [ReactiveFormsModule, SharedModule],
  templateUrl: './tarefa-configuracoes.component.html',
  styleUrl: './tarefa-configuracoes.component.css',
})
export class TarefaConfiguracoesComponent implements OnInit {
  form!: FormGroup;
  carregando = false;
  salvando = false;

  private notificacaoService = inject(NotificacaoService);

  constructor(
    private fb: FormBuilder,
    private service: TarefaService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      receberNotificacaoInternaTarefa: [true],
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
        this.exibirErro(erro, 'Erro ao carregar configuracoes de tarefas.');
      }
    });
  }

  salvar(): void {
    this.salvando = true;

    this.service.atualizarConfiguracoes(this.form.getRawValue()).subscribe({
      next: mensagens => {
        this.salvando = false;
        this.notificacaoService.exibirMensagens(mensagens);
      },
      error: erro => {
        this.salvando = false;
        this.exibirErro(erro, 'Erro ao atualizar configuracoes de tarefas.');
      }
    });
  }

  voltar(): void {
    this.router.navigate(['atron/tarefas']);
  }

  private exibirErro(erro: unknown, mensagemPadrao: string): void {
    if (erro instanceof HttpErrorResponse) {
      this.notificacaoService.exibirMensagem(`Erro ${erro.status}: ${erro.statusText}`, Nivel.Error, 6000);
      return;
    }

    this.notificacaoService.exibirMensagem(mensagemPadrao, Nivel.Error, 6000);
  }
}
