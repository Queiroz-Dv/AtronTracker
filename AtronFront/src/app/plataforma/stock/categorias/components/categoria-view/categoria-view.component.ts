import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { BotaoVoltarComponent } from '../../../../../core/layout/botao-voltar/botao-voltar.component';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { ConfirmacaoExecucaoParams, ConfirmacaoService } from '../../../../../shared/services/confirmacao.service';
import {
  Categoria,
  CategoriaStatus,
  obterDescricaoCategoriaStatus
} from '../../models/categoria.model';
import { CategoriaService } from '../../services/categoria.service';
import { CategoriaHistoricoDialogComponent } from '../categoria-historico-dialog/categoria-historico-dialog.component';

@Component({
  selector: 'c-categoria-view',
  standalone: true,
  imports: [SharedModule, BotaoVoltarComponent],
  templateUrl: './categoria-view.component.html',
  styleUrl: './categoria-view.component.css'
})
export class CategoriaViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  readonly colunas = ['codigo', 'descricao', 'status', 'acoes'];
  readonly dataSource = new MatTableDataSource<Categoria>();
  somenteRemovidas = false;

  private filtroTexto = '';

  constructor(
    private readonly service: CategoriaService,
    private readonly confirmacaoService: ConfirmacaoService,
    private readonly dialog: MatDialog,
    public readonly router: Router
  ) {
    this.dataSource.filterPredicate = categoria => {
      const texto = [
        categoria.codigo,
        categoria.descricao,
        this.descricaoStatus(categoria.status)
      ].join(' ').toLowerCase();

      const correspondeAoTexto = texto.includes(this.filtroTexto);
      const categoriaRemovida = this.estaRemovida(categoria.status);
      const correspondeAoStatus = this.somenteRemovidas
        ? categoriaRemovida
        : !categoriaRemovida;

      return correspondeAoTexto && correspondeAoStatus;
    };

    this.aplicarFiltros();
  }

  ngAfterViewInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.service.obterTodos().subscribe(categorias => {
      this.dataSource.data = categorias;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  filtrar(event: Event): void {
    this.filtroTexto = (event.target as HTMLInputElement).value
      .trim()
      .toLowerCase();
    this.aplicarFiltros();
  }

  alternarFiltroRemovidas(): void {
    this.somenteRemovidas = !this.somenteRemovidas;
    this.aplicarFiltros();
  }

  editar(codigo: string): void {
    this.router.navigate(['/atron/categorias/editar', codigo]);
  }

  alterarStatus(categoria: Categoria): void {
    const ativar = categoria.status !== CategoriaStatus.Ativo;
    const acao = ativar ? 'ativar' : 'inativar';
    const params: ConfirmacaoExecucaoParams = {
      titulo: `Confirmar ${ativar ? 'ativação' : 'inativação'}`,
      mensagem: `Tem certeza que deseja ${acao} ${categoria.codigo} - ${categoria.descricao}?`,
      textoBotaoConfirmar: ativar ? 'Ativar' : 'Inativar',
      operacao$: this.service.alterarStatus(categoria.codigo, ativar),
      onSuccess: () => this.carregar()
    };

    this.confirmacaoService.confirmarEExecutar(params);
  }

  excluir(categoria: Categoria): void {
    const params: ConfirmacaoExecucaoParams = {
      titulo: 'Confirmar exclusão',
      mensagem: `Tem certeza que deseja excluir ${categoria.codigo} - ${categoria.descricao}?`,
      textoBotaoConfirmar: 'Excluir',
      operacao$: this.service.deletar(categoria.codigo),
      onSuccess: () => this.carregar()
    };

    this.confirmacaoService.confirmarEExecutar(params);
  }

  abrirHistorico(categoria: Categoria): void {
    this.dialog.open(CategoriaHistoricoDialogComponent, {
      width: '920px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      data: { categoria }
    });
  }

  descricaoStatus(status: CategoriaStatus): string {
    return obterDescricaoCategoriaStatus(status);
  }

  estaAtiva(status: CategoriaStatus): boolean {
    return status === CategoriaStatus.Ativo;
  }

  estaRemovida(status: CategoriaStatus): boolean {
    return status === CategoriaStatus.Removido;
  }

  private aplicarFiltros(): void {
    this.dataSource.filter = `${this.filtroTexto}|${this.somenteRemovidas}`;
    this.dataSource.paginator?.firstPage();
  }
}
