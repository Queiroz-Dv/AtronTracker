import { Component, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatSelectChange } from '@angular/material/select';
import { finalize, switchMap } from 'rxjs';
import { AcessoService } from '../../../core/services/acesso.service';
import { Nivel, NotificacaoService } from '../../../core/services/notification.service';
import { WorkspaceContextoService } from '../../../core/services/workspace-contexto.service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { WorkspaceResponse } from '../../../shared/models/response/registrar-response.model';

@Component({
  selector: 'atron-workspace-seletor',
  templateUrl: './workspace-seletor.component.html',
  styleUrl: './workspace-seletor.component.css',
  imports: [SharedModule]
})
export class WorkspaceSeletorComponent implements OnInit {
  readonly workspaces = signal<WorkspaceResponse[]>([]);
  readonly carregando = signal(true);
  readonly selecionando = signal(false);
  readonly workspaceAtual = toSignal(this.workspaceContextoService.workspaceAtual$, {
    initialValue: null
  });

  constructor(
    private workspaceContextoService: WorkspaceContextoService,
    private acessoService: AcessoService,
    private notificacaoService: NotificacaoService
  ) { }

  ngOnInit(): void {
    this.workspaceContextoService.listar()
      .pipe(finalize(() => this.carregando.set(false)))
      .subscribe({
        next: workspaces => this.workspaces.set(workspaces),
        error: () => this.notificacaoService.exibirMensagem(
          'Não foi possível carregar os workspaces.',
          Nivel.Error
        )
      });
  }

  selecionar(event: MatSelectChange): void {
    const workspaceId = event.value as number;

    if (!workspaceId || workspaceId === this.workspaceAtual()?.id || this.selecionando()) {
      return;
    }

    this.selecionando.set(true);
    this.workspaceContextoService.selecionar(workspaceId)
      .pipe(switchMap(() => this.acessoService.recarregarSessaoAtual()))
      .pipe(finalize(() => this.selecionando.set(false)))
      .subscribe({
        next: sessao => this.notificacaoService.exibirMensagem(
          `Workspace ${sessao.workspaceAtual?.nome ?? 'selecionado'} selecionado.`,
          Nivel.Sucesso
        ),
        error: () => this.notificacaoService.exibirMensagem(
          'Não foi possível selecionar o workspace.',
          Nivel.Error
        )
      });
  }
}
