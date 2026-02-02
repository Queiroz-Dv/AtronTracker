import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { PerfilDeAcessoViewItem } from './perfil-de-acesso-view-datasource';
import { Router } from '@angular/router';
import { PerfilDeAcessoService } from '../../services/perfil-de-acesso.service';
import { BotaoVoltarComponent } from '../../../../core/layout/botao-voltar/botao-voltar.component';
import { SharedModule } from '../../../../shared/modules/shared.module';

@Component({
  selector: 'c-perfil-de-acesso-view',
  templateUrl: './perfil-de-acesso-view.component.html',
  imports: [SharedModule, BotaoVoltarComponent],
  styleUrls: ['../perfil-de-acesso.component.css']
})
export class PerfilDeAcessoViewComponent implements AfterViewInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<PerfilDeAcessoViewItem>;
  dataSource = new MatTableDataSource<PerfilDeAcessoViewItem>();

  constructor(public router: Router, private service: PerfilDeAcessoService) { }

  displayedColumns = ['codigo', 'descricao', 'quantidadeDeModulos', 'acoes'];

  ngAfterViewInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.service.obterTodos().subscribe(perfis => {
      const perfilViewItem: PerfilDeAcessoViewItem[] = perfis.map(perfil => ({
        codigo: perfil.codigo,
        descricao: perfil.descricao,
        quantidadeDeModulos: perfil.modulos.length
      }));

      this.dataSource = new MatTableDataSource(perfilViewItem);
      this.dataSource.paginator = this.paginator;
      this.table.dataSource = this.dataSource;
    });
  }

  editar(codigo: string): void {
    this.router.navigate(['atron/perfil-de-acesso/editar', codigo]);
  }

  excluir(codigo: string): void {
    if (confirm('Deseja realmente excluir?')) {
      this.service.deletar(codigo).subscribe(() => this.carregar());
    }
  }
}