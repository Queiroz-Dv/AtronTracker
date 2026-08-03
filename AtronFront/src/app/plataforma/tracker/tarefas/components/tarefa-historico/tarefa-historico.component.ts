import { Component, Input } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
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
  totalItens = 0;
  paginaAtual = 0;
  tamanhoPagina = 5;

  constructor(private readonly tarefaService: TarefaService) {}

  alternar(): void {
    this.aberto = !this.aberto;

    if (this.aberto && !this.carregado) {
      this.carregar();
    }
  }

  alterarPagina(evento: PageEvent): void {
    this.paginaAtual = evento.pageIndex;
    this.tamanhoPagina = evento.pageSize;
    this.carregar();
  }

  tentarNovamente(): void {
    this.carregar();
  }

  private carregar(): void {
    this.carregando = true;
    this.erroAoCarregar = false;

    this.tarefaService
      .obterHistorico(this.tarefaId, this.paginaAtual + 1, this.tamanhoPagina)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: pagina => {
          this.dataSource.data = pagina.itens;
          this.totalItens = pagina.totalItens;
          this.carregado = true;
        },
        error: () => {
          this.dataSource.data = [];
          this.totalItens = 0;
          this.erroAoCarregar = true;
        }
      });
  }
}
