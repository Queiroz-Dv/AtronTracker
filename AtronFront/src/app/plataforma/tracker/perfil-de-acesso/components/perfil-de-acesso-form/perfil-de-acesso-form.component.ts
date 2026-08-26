import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { ModuloModel } from '../../../../../features/navegacao/modulos/interfaces/modulo.interface';

@Component({
  selector: 'c-perfil-de-acesso-form',
  imports: [SharedModule, ReactiveFormsModule, MatCheckboxModule],
  templateUrl: './perfil-de-acesso-form.component.html',
  styleUrls: ['../perfil-de-acesso.component.css']
})

export class PerfilDeAcessoFormComponent implements OnInit, OnChanges {
  @Input() form!: FormGroup;
  @Input() todosModulos: ModuloModel[] = [];
  @Input() modulosDoPerfil: ModuloModel[] = [];
  @Input() codigoInformado = false;
  @Input() perfilConsultado = false;
  @Input() perfilEncontrado = false;
  @Input() codigoExistente = false;

  @ViewChild(MatPaginator) set paginator(value: MatPaginator) {
    this.dataSource.paginator = value;
  }

  @ViewChild(MatSort) set sort(value: MatSort) {
    this.dataSource.sort = value;
  }

  dataSource = new MatTableDataSource<ModuloModel>([]);
  buscaModuloControl = new FormControl('');
  columnsToDisplay = ['selecionar', 'moduloCodigo', 'moduloDescricao'];

  ngOnInit(): void {
    this.dataSource.filterPredicate = (modulo, filtro) => {
      const termo = filtro.trim().toLowerCase();
      return modulo.codigo.toLowerCase().includes(termo)
        || modulo.descricao.toLowerCase().includes(termo);
    };

    this.buscaModuloControl.valueChanges.subscribe(() => this.atualizarTabela());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['codigoInformado'] && !this.codigoInformado) {
      this.buscaModuloControl.setValue('', { emitEvent: false });
    }

    this.atualizarTabela();
  }

  get deveExibirBusca(): boolean {
    return !this.codigoExistente;
  }

  get deveExibirGrid(): boolean {
    return !this.codigoExistente && this.dataSource.data.length > 0;
  }

  get mensagemEstado(): string {
    if (this.codigoExistente) {
      return 'O código informado já existe.';
    }

    if (this.perfilEncontrado && this.modulosDoPerfil.length === 0 && !this.obterTermoBusca()) {
      return 'Perfil encontrado, mas ainda não possui módulos relacionados.';
    }

    if (this.perfilConsultado && !this.perfilEncontrado && this.todosModulos.length === 0) {
      return 'Nenhum módulo disponível para montar o perfil.';
    }

    if (this.obterTermoBusca() && this.dataSource.data.length === 0) {
      return 'Nenhum módulo encontrado para a busca informada.';
    }

    if (!this.todosModulos.length) {
      return 'Nenhum módulo disponível para montar o perfil.';
    }

    return '';
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

  private atualizarTabela(): void {
    if (this.codigoExistente) {
      this.dataSource.data = [];
      return;
    }

    const termoBusca = this.obterTermoBusca();
    this.dataSource.data = this.todosModulos;
    this.dataSource.filter = termoBusca;

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  private obterTermoBusca(): string {
    return (this.buscaModuloControl.value ?? '').trim().toLowerCase();
  }
}
