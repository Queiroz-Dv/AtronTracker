import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { SalarioService } from '../../services/salario.service';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { SalarioViewData } from '../../models/salario-view-data.model';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { BotaoVoltarComponent } from "../../../../core/layout/botao-voltar/botao-voltar.component";
import { MatSort } from '@angular/material/sort';

@Component({
  selector: 'c-salario-view',
  templateUrl: './salario-view.component.html',
  imports: [ReactiveFormsModule, SharedModule, BotaoVoltarComponent],
})

export class SalarioViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  dataSource: MatTableDataSource<SalarioViewData>;
  route = inject(ActivatedRoute);

  colunas = ['usuario', 'cargo', 'departamento', 'salario', 'acoes'];

  constructor(
    private service: SalarioService,
    public router: Router) { }

  ngAfterViewInit() {
    this.carregar();
  }

  carregar(): void {
    this.service.listar().subscribe(salarios => {
      const salariosViewData: SalarioViewData[] = salarios.map(salario => ({
        id: salario.id,
        usuarioDescricao: `${salario.usuario.codigo} - ${salario.usuario.nome} ${salario.usuario.sobrenome}`,
        cargoDescricao: formatLabel(salario.usuario.cargo?.codigo, salario.usuario.cargo?.descricao),
        departamentoDescricao: formatLabel(salario.usuario.departamento?.codigo, salario.usuario.departamento?.descricao),
        salarioMensal: salario.salarioMensal
      }));

      this.dataSource = new MatTableDataSource(salariosViewData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  editar(id: number): void {
    this.router.navigate(['atron/salarios/editar', id]);
  }

  excluir(id: number): void {
    if (confirm('Deseja realmente excluir?')) {
      this.service.deletar(id).subscribe(() => this.carregar());
    }
  }
}
