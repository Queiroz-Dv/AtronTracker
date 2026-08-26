import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { EMPTY, Subject, catchError, exhaustMap, finalize, merge, of, takeWhile, tap, timer } from 'rxjs';
import { Nivel, NotificacaoService } from '../../../../../../core/services/notification.service';
import { SharedModule } from '../../../../../../shared/modules/shared.module';
import {
  ProcessamentoProduto,
  ProcessamentoProdutoStatus,
  calcularProgressoProcessamento,
  descreverStatusProcessamento,
  processamentoFinalizado
} from '../../models/processamento-produto.model';
import { ProcessamentoProdutoService } from '../../services/processamento-produto.service';

@Component({
  selector: 'c-processamento-produto-detalhe',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './processamento-produto-detalhe.component.html',
  styleUrl: './processamento-produto-detalhe.component.css'
})
export class ProcessamentoProdutoDetalheComponent implements OnInit {
  readonly Status = ProcessamentoProdutoStatus;
  readonly descreverStatus = descreverStatusProcessamento;
  processamento?: ProcessamentoProduto;
  carregando = false;

  private readonly destroyRef = inject(DestroyRef);
  private readonly notificacao = inject(NotificacaoService);
  private readonly atualizar$ = new Subject<void>();
  private processamentoId = 0;

  constructor(
    private readonly service: ProcessamentoProdutoService,
    private readonly route: ActivatedRoute,
    public readonly router: Router
  ) { }

  ngOnInit(): void {
    this.processamentoId = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isInteger(this.processamentoId) || this.processamentoId <= 0) {
      this.router.navigate(['/atron/produtos/processamentos']);
      return;
    }

    merge(of(undefined), timer(5000, 5000), this.atualizar$).pipe(
      exhaustMap(() => this.obterProcessamento()),
      tap(processamento => this.processamento = processamento),
      takeWhile(processamento => !processamentoFinalizado(processamento.status), true),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();
  }

  atualizar(): void {
    this.atualizar$.next();
  }

  abrirProdutos(): void {
    if (!this.processamento?.loteProdutoCodigo) return;
    this.router.navigate(['/atron/produtos'], {
      queryParams: { lote: this.processamento.loteProdutoCodigo }
    });
  }

  get progresso(): number {
    return this.processamento ? calcularProgressoProcessamento(this.processamento) : 0;
  }

  get pollingAtivo(): boolean {
    return !!this.processamento && !processamentoFinalizado(this.processamento.status);
  }

  private obterProcessamento() {
    this.carregando = true;
    return this.service.obterPorId(this.processamentoId).pipe(
      catchError(() => {
        this.notificacao.exibirMensagem(
          'Não foi possível atualizar o processamento.',
          Nivel.Error
        );
        return EMPTY;
      }),
      finalize(() => this.carregando = false)
    );
  }
}
