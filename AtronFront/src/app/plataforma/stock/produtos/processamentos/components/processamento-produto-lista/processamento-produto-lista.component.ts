import { AfterViewInit, Component, ViewChild, inject } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { BotaoVoltarComponent } from '../../../../../../core/layout/botao-voltar/botao-voltar.component';
import { Nivel, NotificacaoService } from '../../../../../../core/services/notification.service';
import { SharedModule } from '../../../../../../shared/modules/shared.module';
import {
  FILTROS_PROCESSAMENTO_INICIAIS,
  ProcessamentoProdutoFiltros,
  processamentoCorrespondeAosFiltros
} from '../../models/processamento-produto-filtros';
import {
  ProcessamentoProduto,
  ProcessamentoProdutoStatus,
  descreverStatusProcessamento
} from '../../models/processamento-produto.model';
import { ProcessamentoProdutoService } from '../../services/processamento-produto.service';

@Component({
  selector: 'c-processamento-produto-lista',
  standalone: true,
  imports: [SharedModule, BotaoVoltarComponent],
  templateUrl: './processamento-produto-lista.component.html',
  styleUrl: './processamento-produto-lista.component.css'
})
export class ProcessamentoProdutoListaComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  readonly colunas = ['codigoBase', 'status', 'lote', 'acoes'];
  readonly dataSource = new MatTableDataSource<ProcessamentoProduto>();
  readonly Status = ProcessamentoProdutoStatus;
  readonly descreverStatus = descreverStatusProcessamento;
  carregando = false;

  private readonly notificacao = inject(NotificacaoService);
  private filtros: ProcessamentoProdutoFiltros = { ...FILTROS_PROCESSAMENTO_INICIAIS };

  constructor(
    private readonly service: ProcessamentoProdutoService,
    public readonly router: Router
  ) {
    this.dataSource.filterPredicate = processamentoCorrespondeAosFiltros;
  }

  ngAfterViewInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.service.obterTodos().subscribe({
      next: processamentos => {
        this.dataSource.data = processamentos;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.aplicarFiltros();
        this.carregando = false;
      },
      error: () => {
        this.carregando = false;
        this.notificacao.exibirMensagem(
          'Não foi possível carregar os processamentos.',
          Nivel.Error
        );
      }
    });
  }

  filtrarTexto(event: Event): void {
    this.filtros.texto = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.aplicarFiltros();
  }

  filtrarStatus(status: ProcessamentoProdutoStatus | ''): void {
    this.filtros.status = status === '' ? null : status;
    this.aplicarFiltros();
  }

  abrirDetalhe(id: number): void {
    this.router.navigate(['/atron/produtos/processamentos', id]);
  }

  private aplicarFiltros(): void {
    this.dataSource.filter = JSON.stringify(this.filtros);
    this.dataSource.paginator?.firstPage();
  }
}
