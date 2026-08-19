import { Component, Input, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { finalize } from 'rxjs';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { TarefaMovimentacao } from '../../models/tarefa-movimentacao.model';
import { TarefaService } from '../../services/tarefa.service';

@Component({
  selector: 'c-tarefa-historico',
  imports: [SharedModule],
  templateUrl: './tarefa-historico.component.html',
  styleUrl: './tarefa-historico.component.css'
})
export class TarefaHistoricoComponent {
  @Input({ required: true }) tarefaId!: number;

  readonly colunas = ['movimento', 'detalhes', 'responsavel', 'dataOcorrencia'];
  readonly dataSource = new MatTableDataSource<TarefaMovimentacao>([]);
  aberto = false;
  carregando = false;
  carregado = false;
  erroAoCarregar = false;

  @ViewChild(MatPaginator)
  set paginator(paginator: MatPaginator | undefined) {
    this.dataSource.paginator = paginator ?? null;
  }

  constructor(private readonly tarefaService: TarefaService) {}

  alternar(): void {
    this.aberto = !this.aberto;

    if (this.aberto && !this.carregado) {
      this.carregar();
    }
  }

  tentarNovamente(): void {
    this.carregar();
  }

  private carregar(): void {
    this.carregando = true;
    this.erroAoCarregar = false;

    this.tarefaService
      .obterHistorico(this.tarefaId)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: movimentacoes => {
          this.dataSource.data = movimentacoes;
          this.dataSource.paginator?.firstPage();
          this.carregado = true;
        },
        error: () => {
          this.dataSource.data = [];
          this.erroAoCarregar = true;
        }
      });
  }
}
