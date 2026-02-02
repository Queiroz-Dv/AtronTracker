import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CargoService } from '../../services/cargo.service';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CargoFormComponent } from "../cargo-form/cargo-form.component";
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { CargoRequest } from '../../models/request/cargo-request.model';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'c-cargo-edit',
  imports: [ReactiveFormsModule, SharedModule, CargoFormComponent],
  templateUrl: './cargo-edit.component.html',
})

export class CargoEditComponent implements OnInit {
  form!: FormGroup;
  departamentos!: Departamento[];
  codigo?: string | number;

  private notificacaoService = inject(NotificacaoService);

  constructor(
    private fb: FormBuilder,
    private service: CargoService,
    private departamentoService: DepartamentosService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]],
      descricao: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      departamentoCodigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]]
    });

    this.departamentoService.obterTodos().subscribe(dpt => this.departamentos = dpt);

    this.codigo = this.route.snapshot.paramMap.get('codigo');
    if (this.codigo) {
      this.form.get('codigo')?.disable();
      this.service.obterPorCodigo(this.codigo).subscribe({
        next: (crg) => this.form.patchValue(crg),
        error: () => {
          this.notificacaoService.exibirMensagem("Erro ao carregar dados para edição.", Nivel.Error);
          this.router.navigate(["atron/cargos"]);
        }
      });
    }
  }

  async salvar() {

    if (this.form.invalid) {
      this.notificacaoService.exibirMensagem("Formulário inválido. Verifique os campos.", Nivel.Error);
      return;
    }

    const dadosForm = this.form.getRawValue();
    const codigoParaSalvar = this.codigo ? this.codigo : dadosForm.codigo;
    const cargosPayload = new CargoRequest(codigoParaSalvar, dadosForm.descricao, dadosForm.departamentoCodigo);

    const request = this.codigo
      ? this.service.atualizar(this.codigo, cargosPayload)
      : this.service.gravar(cargosPayload);

   request.subscribe({
      next: (resposta: Mensagem[]) => {
        this.notificacaoService.exibirMensagens(resposta);
        if (!resposta.some(m => m.nivel === Nivel.Error)) {
          this.router.navigate(['atron/cargos']);
        }
      },
      error: (erro: any) => {
        if (erro && erro.mensagensApi) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
        } else if (erro instanceof HttpErrorResponse) {
          this.notificacaoService.exibirMensagem(`Erro ${erro.status}: ${erro.statusText}`, Nivel.Error, 6000);
        } else {
          this.notificacaoService.exibirMensagem('Ocorreu um erro inesperado.', Nivel.Error, 6000);
        }
      }
    });
  }
}