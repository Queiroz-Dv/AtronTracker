import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { TarefaService } from '../../services/tarefa.service';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { TarefaViewData } from '../../models/tarefa-view-data.model';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { BotaoVoltarComponent } from "../../../../core/layout/botao-voltar/botao-voltar.component";
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'c-tarefa-view',
  templateUrl: './tarefa-view.component.html',
  imports: [SharedModule, ReactiveFormsModule, BotaoVoltarComponent],
})

export class TarefaViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  dataSource: MatTableDataSource<TarefaViewData>;

  route = inject(ActivatedRoute);
  colunas = [
    'usuario',
    'cargo',
    'departamento',
    'titulo',
    'dataInicial',
    'dataFinal',
    'estado',
    'acoes'];

  constructor(private service: TarefaService, public router: Router) { }

  ngAfterViewInit() { this.carregar(); }

  carregar() {
    this.service.obterTodasTarefasRelacionadas().subscribe(tarefas => {
      const tarefaViewData = tarefas.map(trf => ({
        id: trf.id,
        nomeDoUsuario: `${trf.usuario.codigo} - ${trf.usuario.nome} ${trf.usuario.sobrenome}`,
        cargoDescricao: formatLabel(trf.usuario.cargo?.codigo, trf.usuario.cargo?.descricao),
        departamentoDescricao: formatLabel(trf.usuario.departamento?.codigo, trf.usuario.departamento?.descricao),
        descricaoDoEstadoDaTarefa: trf.estadoDaTarefa.descricao,
        titulo: trf.titulo,
        dataInicial: trf.dataInicial,
        dataFinal: trf.dataFinal
      }));

      this.dataSource = new MatTableDataSource(tarefaViewData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  editar(id: number): void {
    this.router.navigate(['atron/tarefas/editar', id]);
  }

  excluir(id: number): void {
    if (confirm('Deseja realmente excluir?')) {
      this.service.deletar(id).subscribe(() => this.carregar());
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  formatDate(arg0: any) {
    const full_date = new Date(arg0).toLocaleDateString("pt-BR");
    return full_date;
  }
}