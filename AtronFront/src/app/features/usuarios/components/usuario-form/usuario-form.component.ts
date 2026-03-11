import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { CargoService } from '../../../cargos/services/cargo.service';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { CargoModel } from '../../../cargos/models/cargo.model';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';
import { DateMaskDirective } from '../../../../shared/directives/date-mask.directive';

@Component({
  selector: 'c-usuario-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, DateMaskDirective],
  templateUrl: './usuario-form.component.html',
})

export class UsuarioFormComponent implements OnInit {
  @Input() usuarioForm!: FormGroup;

  // Controles individuais para autocomplete
  departamentoControl = new FormControl<string | Departamento>('');
  cargoControl = new FormControl<string | CargoResponse | CargoModel>('');

  // Dados filtrados para os autocompletes
  departamentosFiltrados: Departamento[] = [];
  cargosFiltrados: CargoResponse[] = [];

  // Backup dos dados originais
  todosDepartamentos: Departamento[] = [];
  todosCargos: CargoResponse[] = [];

  constructor(
    private cargoService: CargoService,
    private departamentoService: DepartamentosService
  ) { }

  ngOnInit(): void {
    this.departamentoService.obterTodos().subscribe(deps => {
      this.todosDepartamentos = deps;
      this.departamentosFiltrados = this.todosDepartamentos;

      const dptCodigo = this.usuarioForm.get('departamentoCodigo')?.value;
      const selecionado = this.todosDepartamentos.find(d => d.codigo === dptCodigo);
      this.departamentoControl.setValue(selecionado || '');
    });

    this.cargoService.obterTodos().subscribe(crgs => {
      this.todosCargos = crgs;
      this.cargosFiltrados = this.todosCargos;

      const cargoCodigo = this.usuarioForm.get('cargoCodigo')?.value;
      const selecionado = this.todosCargos.find(c => c.codigo === cargoCodigo);
      this.cargoControl.setValue(selecionado || '');
    });

    this.departamentoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        // Usuário está digitando - aplica filtro de busca
        this.departamentosFiltrados = this._filtrarDepartamentos(value);
      } else if (value) {
        // Usuário selecionou um item
        const departamento = value as Departamento;
        this.cargosFiltrados = this.todosCargos.filter(c => c.departamentoCodigo === departamento.codigo);
        this.usuarioForm.patchValue({ departamentoCodigo: departamento.codigo });
      } else {
        this.usuarioForm.patchValue({ departamentoCodigo: null });
        this.cargosFiltrados = this.todosCargos;
      }
    });

    this.cargoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        // Usuário está digitando - aplica filtro de busca
        this.cargosFiltrados = this._filtrarCargos(value);
      } else if (value) {
        // Usuário selecionou um item
        const cargo = value as CargoModel;
        this.departamentosFiltrados = this.todosDepartamentos.filter(d => d.codigo === cargo.departamentoCodigo);
        this.usuarioForm.patchValue({ cargoCodigo: cargo.codigo });
      } else {
        this.usuarioForm.patchValue({ cargoCodigo: null });
        this.departamentosFiltrados = this.todosDepartamentos;
      }
    });
  }

  private _filtrarDepartamentos(valor: string): Departamento[] {
    const filtro = valor.toLowerCase();
    return this.todosDepartamentos.filter(dep => 
      dep.descricao.toLowerCase().includes(filtro) || 
      dep.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  private _filtrarCargos(valor: string): CargoResponse[] {
    const filtro = valor.toLowerCase();
    return this.todosCargos.filter(crg => 
      crg.descricao.toLowerCase().includes(filtro) || 
      crg.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  exibirDepartamentoDescricao = (dpt: Departamento) => dpt?.descricao ?? '';
  exibirCargoDescricao = (crg: CargoModel) => crg?.descricao ?? '';
}
