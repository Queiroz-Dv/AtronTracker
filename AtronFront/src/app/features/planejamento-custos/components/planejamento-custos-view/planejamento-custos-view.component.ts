import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { BotaoVoltarComponent } from '../../../../core/layout/botao-voltar/botao-voltar.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { filter, switchMap } from 'rxjs';
import { ConfirmacaoDialogComponent, ConfirmacaoDialogData } from '../../../../shared/components/confirmacao-dialog/confirmacao-dialog.component';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { PlanejamentoCustoResponse } from '../../models/response/planejamento-custo-response.model';
import { PlanejamentoCustoRelatorioImpressaoService } from '../../services/planejamento-custo-relatorio-impressao.service';
import { PlanejamentoCustoService } from '../../services/planejamento-custo.service';
import { PlanejamentoCustosDetalheDialogComponent } from '../planejamento-custos-detalhe-dialog/planejamento-custos-detalhe-dialog.component';

@Component({
  selector: 'c-planejamento-custos-view',
  standalone: true,
  imports: [SharedModule, ReactiveFormsModule, BotaoVoltarComponent],
  templateUrl: './planejamento-custos-view.component.html',
  styleUrls: ['../../planejamento-custos.component.css']
})
export class PlanejamentoCustosViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  dataSource: MatTableDataSource<PlanejamentoCustoResponse> = new MatTableDataSource();
  colunas = ['codigo', 'descricao', 'ano', 'departamento', 'valorMinimo', 'valorTeto', 'modo', 'acoes'];
  imprimindoCodigo?: string;

  constructor(
    private service: PlanejamentoCustoService,
    private impressaoService: PlanejamentoCustoRelatorioImpressaoService,
    private dialog: MatDialog,
    private notificacaoService: NotificacaoService,
    public router: Router) { }

  ngAfterViewInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.service.obterTodos().subscribe(planejamentos => {
      this.dataSource.data = planejamentos;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  editar(codigo: string): void {
    this.router.navigate(['atron/planejamento-custos/editar', codigo]);
  }

  detalhar(planejamento: PlanejamentoCustoResponse): void {
    this.dialog.open(PlanejamentoCustosDetalheDialogComponent, {
      width: '720px',
      maxWidth: '96vw',
      panelClass: 'planning-detail-panel',
      data: planejamento
    });
  }

  imprimirPlanejamento(codigo: string): void {
    this.imprimindoCodigo = codigo;
    this.service.obterRelatorioPlanejamentoImpressao(codigo).subscribe({
      next: htmlRelatorio => {
        this.impressaoService.imprimir(htmlRelatorio);
        this.imprimindoCodigo = undefined;
      },
      error: erro => {
        this.imprimindoCodigo = undefined;
        if (erro?.mensagensApi?.length) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
          return;
        }

        this.notificacaoService.exibirMensagem('Nao foi possivel imprimir o relatorio do planejamento.', Nivel.Error);
      }
    });
  }

  excluir(codigo: string): void {
    const planejamento = this.dataSource.data.find(plc => plc.codigo === codigo);
    const nomePlanejamento = planejamento ? `${planejamento.codigo} - ${planejamento.descricao}` : `o registro ${codigo}`;

    const dialogData: ConfirmacaoDialogData = {
      titulo: 'Confirmar Exclusão',
      mensagem: `Tem certeza que deseja excluir ${nomePlanejamento}?`,
      textoBotaoConfirmar: 'Excluir',
      textoBotaoCancelar: 'Cancelar'
    };

    const dialogRef = this.dialog.open(ConfirmacaoDialogComponent, {
      width: '500px',
      data: dialogData
    });

    dialogRef.afterClosed().pipe(
      filter(resultado => resultado === true),
      switchMap(() => this.service.deletar(codigo))
    ).subscribe({
      next: (resposta: Mensagem[]) => {
        this.notificacaoService.exibirMensagens(resposta);
        this.carregar();
      },
      error: (erro: any) => {
        if (erro && erro.mensagensApi) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
        } else {
          this.notificacaoService.exibirMensagem('Ocorreu um erro ao tentar excluir o planejamento.', Nivel.Error);
        }
      }
    });
  }

  formatarMoeda(valor: number): string {
    return (valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
  }
}
