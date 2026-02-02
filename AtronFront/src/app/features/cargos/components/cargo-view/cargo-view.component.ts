import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { CargoService } from '../../services/cargo.service';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { CargoModel } from '../../models/cargo.model';
import { BotaoVoltarComponent } from "../../../../core/layout/botao-voltar/botao-voltar.component";
import { MatSort } from '@angular/material/sort';
import { CargoResponse } from '../../models/response/cargo-response.model';
import { MatDialog } from '@angular/material/dialog';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { ConfirmacaoDialogComponent, ConfirmacaoDialogData } from '../../../../shared/components/confirmacao-dialog/confirmacao-dialog.component';
import { filter, switchMap } from 'rxjs';

@Component({
  selector: 'c-cargos-view',
  templateUrl: './cargo-view.component.html',
  imports: [ReactiveFormsModule, SharedModule, BotaoVoltarComponent],
})

export class CargosViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  dataSource: MatTableDataSource<CargoResponse> = new MatTableDataSource();

  route = inject(ActivatedRoute);
  departamentos: Departamento[] = [];
  colunas = ['codigo', 'descricao', 'departamento', 'acoes'];

  constructor(
    private service: CargoService,
    private dialog: MatDialog,
    private notificacaoService: NotificacaoService,
    public router: Router) { }

  ngAfterViewInit(): void {
    this.carregar();
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  carregar() {
    this.service.obterTodos().subscribe(crg => {
      this.dataSource.data = crg;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  editar(codigo: string): void {
    this.router.navigate(['atron/cargos/editar', codigo]);
  }

  excluir(codigo: string): void {
    const cargo = this.dataSource.data.find(d => d.codigo === codigo);
    const nomeCargo = cargo ? `${cargo.codigo} - ${cargo.descricao}` : `o registro ${codigo}`;

    const dialogData: ConfirmacaoDialogData = {
      titulo: 'Confirmar Exclusão',
      mensagem: `Tem certeza que deseja excluir ${nomeCargo}?`,
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
          this.notificacaoService.exibirMensagem('Ocorreu um erro ao tentar excluir o registro.', Nivel.Error);
        }
      }
    });
  }
}