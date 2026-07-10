import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SalarioService } from '../../services/salario.service';
import { SalarioFormComponent } from '../salario-form/salario-form.component';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { SalarioRequest } from '../../models/request/salario-request';

@Component({
  selector: 'c-salario-edit',
  imports: [ReactiveFormsModule, SharedModule, SalarioFormComponent],
  templateUrl: './salario-edit.component.html',
})

export class SalarioEditComponent implements OnInit {
  @ViewChild(SalarioFormComponent)
  salarioFormComponent: SalarioFormComponent;
  form!: FormGroup;
  id?: number;

  constructor(
    private fb: FormBuilder,
    private service: SalarioService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [null],
      salarioMensal: [null, [Validators.required, Validators.min(1500)]],
      usuarioCodigo: [null, Validators.required],
      usuarioNome: [''],
      cargoDescricao: [''],
      departamentoDescricao: [''],
      mesDescricao: [''],
      mesId: [null, Validators.required],
      ano: [null, Validators.required]
    });

    this.id = +(this.route.snapshot.paramMap.get('id') || 0);
    if (this.id) {
      this.service.obterPorId(this.id).subscribe(sal => {
        this.form.patchValue({
          ...sal,
          usuarioCodigo: sal.usuarioCodigo,
          mesId: sal.mesId,
          ano: sal.ano
        });

        setTimeout(() => this.salarioFormComponent.desabilitarUsuarioSelect());
      });
    }
  }

  salvar() {
    const salarioRawValue = this.form.getRawValue();
    const salarioPayload: SalarioRequest = {
      id: this.id || 0,
      salarioMensal: salarioRawValue.salarioMensal,
      usuarioCodigo: salarioRawValue.usuarioCodigo,
      mesId: salarioRawValue.mesId,
      ano: salarioRawValue.ano
    };

    const operacao = this.id
      ? this.service.atualizar(this.id, salarioPayload)
      : this.service.gravar(salarioPayload);

    operacao.subscribe(() => this.router.navigate(['atron/salarios']));
  }
}