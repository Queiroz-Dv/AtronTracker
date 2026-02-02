import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, ReactiveFormsModule, FormControl } from '@angular/forms';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
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

  constructor(private departamentoService: DepartamentosService) { }

  ngOnInit(): void {
    this.departamentoService.obterTodos().subscribe(deps => {
      this.departamentos = deps;

      const dptCodigo = this.cargoForm.get('departamentoCodigo')?.value;
      const selecionado = this.departamentos.find(d => d.codigo === dptCodigo);
      this.departamentoControl.setValue(selecionado);
    });

    this.departamentoControl.valueChanges.subscribe((value: string | Departamento) => {
      const dpt = typeof value === 'string' ? null : value;
      this.cargoForm.patchValue({
        departamentoCodigo: dpt?.codigo || null
      });
    });
  }

  exibirDescricao = (dpt: Departamento) => dpt?.descricao || '';
}