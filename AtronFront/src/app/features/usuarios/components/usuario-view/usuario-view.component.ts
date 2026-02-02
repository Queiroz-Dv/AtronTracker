import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { UsuarioService } from '../../services/usuario.service';
import { ReactiveFormsModule } from '@angular/forms';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { BotaoVoltarComponent } from "../../../../core/layout/botao-voltar/botao-voltar.component";
import { MatSort } from '@angular/material/sort';

class UsuarioViewData {
  codigoOriginal: string;
  codigo: string;
  cargo: string;
  departamento: string;
}

@Component({
  selector: 'c-usuario-view',
  templateUrl: './usuario-view.component.html',
  imports: [SharedModule, ReactiveFormsModule, BotaoVoltarComponent],
})
export class UsuarioViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  dataSource = new MatTableDataSource<UsuarioViewData>();
  route = inject(ActivatedRoute);
  colunas = ['codigo', 'cargo', 'departamento', 'acoes'];

  constructor(private service: UsuarioService, public router: Router) { }

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
    this.service.obterTodosUsuariosInformados().subscribe(usuarios => {

      const usuarioViewData = usuarios.map(usr => ({
        codigoOriginal: usr.codigo,
        codigo: `${usr.codigo} - ${usr.nome} ${usr.sobrenome}`,
        cargo: formatLabel(usr.cargoCodigo, usr.cargoDescricao),
        departamento: formatLabel(usr.departamentoCodigo, usr.departamentoDescricao)
      }));

      this.dataSource = new MatTableDataSource(usuarioViewData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    })
  }

  editar(codigo: string): void {
    this.router.navigate(['atron/usuarios/editar', codigo]);
  }

  excluir(codigo: string): void {
    if (confirm('Deseja realmente excluir?')) {
      this.service.deletar(codigo).subscribe(() => this.carregar());
    }
  }
}

