import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { BotaoVoltarComponent } from '../../../../../core/layout/botao-voltar/botao-voltar.component';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import {
  Produto,
  ProdutoStatus,
  obterDescricaoProdutoStatus
} from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';
import { ProdutoHistoricoDialogComponent } from '../produto-historico-dialog/produto-historico-dialog.component';
import {
  FILTROS_PRODUTO_INICIAIS,
  ProdutoFiltros,
  produtoCorrespondeAosFiltros
} from './produto-filtros';

@Component({
  selector: 'c-produto-view',
  standalone: true,
  imports: [SharedModule, BotaoVoltarComponent],
  templateUrl: './produto-view.component.html',
  styleUrl: './produto-view.component.css'
})
export class ProdutoViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  readonly colunas = ['codigo', 'descricao', 'aquisicao', 'status', 'acoes'];
  readonly dataSource = new MatTableDataSource<Produto>();
  readonly ProdutoStatus = ProdutoStatus;
  readonly descricaoStatus = obterDescricaoProdutoStatus;

  private filtros: ProdutoFiltros = { ...FILTROS_PRODUTO_INICIAIS };

  constructor(
    private readonly service: ProdutoService,
    private readonly dialog: MatDialog,
    private readonly route: ActivatedRoute,
    public readonly router: Router
  ) {
    this.dataSource.filterPredicate = produtoCorrespondeAosFiltros;
    this.filtros.lote = this.route.snapshot.queryParamMap.get('lote') ?? '';
  }

  ngAfterViewInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.service.obterTodos().subscribe(produtos => {
      this.dataSource.data = produtos;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.aplicarFiltros();
    });
  }

  filtrarTexto(event: Event): void {
    this.filtros.texto = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.aplicarFiltros();
  }

  filtrarStatus(valor: ProdutoStatus | ''): void {
    this.filtros.status = valor === '' ? null : valor;
    this.aplicarFiltros();
  }

  editar(codigo: string): void {
    this.router.navigate(['/atron/produtos/editar', codigo]);
  }

  abrirHistorico(produto: Produto): void {
    this.dialog.open(ProdutoHistoricoDialogComponent, {
      width: '920px',
      maxWidth: '95vw',
      data: { produto }
    });
  }

  private aplicarFiltros(): void {
    this.dataSource.filter = JSON.stringify(this.filtros);
    this.dataSource.paginator?.firstPage();
  }
}
