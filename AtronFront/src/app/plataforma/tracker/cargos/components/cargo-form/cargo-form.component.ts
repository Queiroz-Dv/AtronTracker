import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, FormControl } from '@angular/forms';
import { ControlErrorComponent } from '../../../../../shared/components/control-error/control-error.component';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { Departamento } from '../../../departamentos/models/departamento.model';

@Component({
  selector: 'c-cargo-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './cargo-form.component.html',
})
export class CargoFormComponent implements OnInit {
  @Input() cargoForm!: FormGroup;
  @Input() departamentos: Departamento[] = [];
  departamentoControl = new FormControl<string | Departamento>('');
  departamentosFiltrados: Departamento[] = [];
  private todosDepartamentos: Departamento[] = [];

  constructor(private departamentoService: DepartamentosService) { }

  ngOnInit(): void {
    this.departamentoService.obterTodos().subscribe(deps => {
      this.todosDepartamentos = deps;
      this.departamentos = deps;
      this.departamentosFiltrados = deps;

      const dptCodigo = this.cargoForm.get('departamentoCodigo')?.value;
      this.atualizarDepartamentoSelecionado(dptCodigo);
    });

    this.cargoForm.get('departamentoCodigo')?.valueChanges.subscribe(val => {
      this.atualizarDepartamentoSelecionado(val);
    });

    this.departamentoControl.valueChanges.subscribe((value: string | Departamento) => {
      if (typeof value === 'string') {
        this.departamentosFiltrados = this.filtrarDepartamentos(value);
        this.cargoForm.patchValue({ departamentoCodigo: null }, { emitEvent: false });
        return;
      }

      if (value) {
        this.departamentosFiltrados = this.todosDepartamentos;
        this.cargoForm.patchValue({ departamentoCodigo: value.codigo }, { emitEvent: false });
        return;
      }

      this.departamentosFiltrados = this.todosDepartamentos;
      this.cargoForm.patchValue({ departamentoCodigo: null }, { emitEvent: false });
    });
  }

  private atualizarDepartamentoSelecionado(codigo: any) {
    if (this.todosDepartamentos) {
      const selecionado = this.todosDepartamentos.find(d => d.codigo === codigo);
      if (this.departamentoControl.value !== selecionado) {
        this.departamentoControl.setValue(selecionado || '', { emitEvent: false });
      }
    }
  }

  private filtrarDepartamentos(valor: string): Departamento[] {
    const filtro = valor.toLowerCase().trim();
    if (!filtro) return this.todosDepartamentos;

    return this.todosDepartamentos.filter(dep =>
      dep.descricao.toLowerCase().includes(filtro)
      || dep.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  exibirDescricao = (dpt: Departamento) => dpt?.descricao || '';
}
