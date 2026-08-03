import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { ControlErrorComponent } from '../../../../../shared/components/control-error/control-error.component';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';

@Component({
  selector: 'c-planejamento-custos-form',
  standalone: true,
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './planejamento-custos-form.component.html',
  styleUrls: ['../../planejamento-custos.component.css']
})
export class PlanejamentoCustosFormComponent implements OnInit {
  @Input() planejamentoForm!: FormGroup;
  @Input() set cargosDisponiveis(cargos: CargoResponse[] | null) {
    this.todosCargos = cargos ?? [];
    this.atualizarCargosFiltrados(this.cargoControl.value);
  }

  departamentoControl = new FormControl<string | Departamento>('');
  cargoControl = new FormControl<string | CargoResponse>('');
  departamentosFiltrados: Departamento[] = [];
  cargosFiltrados: CargoResponse[] = [];
  pageIndex = 0;
  pageSize = 5;
  pageSizeOptions = [5, 10, 25];
  private todosDepartamentos: Departamento[] = [];
  private todosCargos: CargoResponse[] = [];

  constructor(private departamentoService: DepartamentosService) { }

  ngOnInit(): void {
    this.departamentoService.obterTodos().subscribe(deps => {
      this.todosDepartamentos = deps;
      this.departamentosFiltrados = deps;
      this.atualizarDepartamentoSelecionado(this.planejamentoForm.get('departamentoCodigo')?.value);
    });

    this.planejamentoForm.get('departamentoCodigo')?.valueChanges.subscribe(val => {
      this.atualizarDepartamentoSelecionado(val);
      this.pageIndex = 0;
      this.atualizarCargosFiltrados(this.cargoControl.value);
    });

    this.departamentoControl.valueChanges.subscribe((value: string | Departamento) => {
      if (this.planejamentoForm.get('departamentoCodigo')?.disabled) {
        return;
      }

      if (typeof value === 'string') {
        this.departamentosFiltrados = this.filtrarDepartamentos(value);
        if (!value.trim()) {
          this.planejamentoForm.patchValue({ departamentoCodigo: null });
        }
        return;
      }

      if (value) {
        this.departamentosFiltrados = this.todosDepartamentos;
        this.planejamentoForm.patchValue({ departamentoCodigo: value.codigo });
        return;
      }

      this.departamentosFiltrados = this.todosDepartamentos;
      this.planejamentoForm.patchValue({ departamentoCodigo: null });
    });

    this.cargoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.atualizarCargosFiltrados(value);
        return;
      }

      if (!value) {
        this.atualizarCargosFiltrados('');
      }
    });
  }

  private atualizarDepartamentoSelecionado(codigo: any): void {
    const selecionado = this.todosDepartamentos.find(d => d.codigo === codigo);
    if (this.departamentoControl.value !== selecionado) {
      this.departamentoControl.setValue(selecionado || '', { emitEvent: false });
    }

    this.departamentosFiltrados = selecionado ? [selecionado] : this.todosDepartamentos;
  }

  private filtrarDepartamentos(valor: string): Departamento[] {
    const filtro = valor.toLowerCase().trim();
    if (!filtro) return this.todosDepartamentos;

    return this.todosDepartamentos.filter(dep =>
      dep.descricao.toLowerCase().includes(filtro)
      || dep.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  exibirDescricao = (dpt: Departamento | string) =>
    typeof dpt === 'string' ? dpt : dpt ? `${dpt.codigo} - ${dpt.descricao}` : '';

  exibirCargo = (cargo: CargoResponse | string) =>
    typeof cargo === 'string' ? cargo : cargo ? `${cargo.codigo} - ${cargo.descricao}` : '';

  get detalhesCargo(): FormArray {
    return this.planejamentoForm.get('detalhesCargo') as FormArray;
  }

  get deveExibirCargos(): boolean {
    return !this.planejamentoForm.get('apenasDepartamento')?.value;
  }

  get detalhesSelecionados(): { control: AbstractControl, index: number }[] {
    return this.detalhesCargo.controls
      .map((control, index) => ({ control, index }))
      .filter(item => item.control.get('detalhado')?.value);
  }

  get totalDetalhesSelecionados(): number {
    return this.detalhesSelecionados.length;
  }

  get detalhesPaginados(): { control: AbstractControl, index: number }[] {
    const inicio = this.pageIndex * this.pageSize;
    return this.detalhesSelecionados.slice(inicio, inicio + this.pageSize);
  }

  selecionarCargo(cargo: CargoResponse | string | null): void {
    if (!cargo || typeof cargo === 'string') {
      return;
    }

    const index = this.detalhesCargo.controls.findIndex(control => control.get('cargoCodigo')?.value === cargo.codigo);
    if (index < 0) {
      return;
    }

    this.detalhesCargo.at(index).patchValue({ detalhado: true });
    this.cargoControl.setValue('', { emitEvent: false });
    this.pageIndex = Math.floor((this.totalDetalhesSelecionados - 1) / this.pageSize);
    this.atualizarCargosFiltrados('');
  }

  removerCargoDetalhado(index: number): void {
    this.detalhesCargo.at(index).patchValue({
      detalhado: false,
      valorMinimo: null,
      valorTeto: null
    });
    this.ajustarPagina();
    this.atualizarCargosFiltrados(this.cargoControl.value);
  }

  paginaAlterada(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
  }

  private atualizarCargosFiltrados(valor: string | CargoResponse | null): void {
    const departamentoCodigo = this.planejamentoForm?.get('departamentoCodigo')?.value;
    const filtro = typeof valor === 'string' ? valor.toLowerCase().trim() : '';
    const cargosDetalhados = new Set(
      this.planejamentoForm
        ? this.detalhesCargo.controls
        .filter(control => control.get('detalhado')?.value)
        .map(control => control.get('cargoCodigo')?.value)
        : []
    );

    this.cargosFiltrados = this.todosCargos
      .filter(cargo => cargo.departamentoCodigo === departamentoCodigo)
      .filter(cargo => !cargosDetalhados.has(cargo.codigo))
      .filter(cargo =>
        !filtro
        || cargo.descricao.toLowerCase().includes(filtro)
        || cargo.codigo.toLowerCase().includes(filtro)
      )
      .sort((a, b) => a.descricao.localeCompare(b.descricao));
  }

  private ajustarPagina(): void {
    const total = this.totalDetalhesSelecionados;
    const ultimaPagina = Math.max(Math.ceil(total / this.pageSize) - 1, 0);
    if (this.pageIndex > ultimaPagina) {
      this.pageIndex = ultimaPagina;
    }
  }
}
