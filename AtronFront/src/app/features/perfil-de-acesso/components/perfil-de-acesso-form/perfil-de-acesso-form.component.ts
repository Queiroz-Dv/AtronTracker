import { Component, inject, Input, OnInit, ViewChild } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PerfilDeAcessoService } from '../../services/perfil-de-acesso.service';
import { ModuloService } from '../../../modulos/services/modulo.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { ModuloModel } from '../../../modulos/interfaces/modulo.interface';

@Component({
  selector: 'c-perfil-de-acesso-form',
  imports: [SharedModule, ReactiveFormsModule, MatCheckboxModule],
  templateUrl: './perfil-de-acesso-form.component.html',
  styleUrls: ['../perfil-de-acesso.component.css']
})

export class PerfilDeAcessoFormComponent implements OnInit {
  @Input() form!: FormGroup;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<ModuloModel>;
  @Input() todosModulos: ModuloModel[] = [];

  dataSource = new MatTableDataSource<ModuloModel>([])

  columnsToDisplay = ['moduloCodigo', 'moduloDescricao',];
  modulos: ModuloModel[] = [];
  service = inject(PerfilDeAcessoService);
  moduloService = inject(ModuloService);

  ngOnInit(): void {
    this.moduloService.obterTodos().subscribe(mods => {
      this.dataSource = new MatTableDataSource(mods)
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.table.dataSource = this.dataSource;
    })
  }

  onToggleModulo(codigo: string, selecionado: boolean) {
    const modulosControl = this.form.get('modulos');
    const modulosSelecionados = modulosControl?.value || [];

    if (selecionado && !modulosSelecionados.includes(codigo)) {
      modulosControl?.setValue([...modulosSelecionados, codigo]);
    } else if (!selecionado) {
      modulosControl?.setValue(modulosSelecionados.filter((c: string) => c !== codigo));
    }
  }

  estaSelecionado(codigo: string): boolean {
    const selecionados = this.form?.get('modulos')?.value || [];
    return selecionados.includes(codigo);
  }

}