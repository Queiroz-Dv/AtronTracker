import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';
import { CargoService } from '../../../cargos/services/cargo.service';
import { PlanejamentoCustoRequest } from '../../models/request/planejamento-custo-request.model';
import { PlanejamentoCustoCargoResponse } from '../../models/response/planejamento-custo-response.model';
import { PlanejamentoCustoFormularioState } from '../../services/planejamento-custo-formulario-state';
import { PlanejamentoCustoService } from '../../services/planejamento-custo.service';
import { PlanejamentoCustosFormComponent } from '../planejamento-custos-form/planejamento-custos-form.component';

@Component({
  selector: 'c-planejamento-custos-edit',
  standalone: true,
  imports: [ReactiveFormsModule, SharedModule, PlanejamentoCustosFormComponent],
  templateUrl: './planejamento-custos-edit.component.html'
})
export class PlanejamentoCustosEditComponent implements OnInit {
  form!: FormGroup;
  codigo?: string | number;
  anoAtual = new Date().getFullYear();
  todosCargos: CargoResponse[] = [];
  private detalhesCarregados: PlanejamentoCustoCargoResponse[] = [];

  private notificacaoService = inject(NotificacaoService);

  constructor(
    private fb: FormBuilder,
    private service: PlanejamentoCustoService,
    private cargoService: CargoService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: [''],
      descricao: [''],
      ano: [this.anoAtual],
      valorMinimo: [0],
      valorTeto: [0],
      apenasDepartamento: [false],
      departamentoCodigo: [''],
      detalhesCargo: this.fb.array([])
    });

    this.cargoService.obterTodos().subscribe(cargos => {
      this.todosCargos = cargos;
      this.sincronizarDetalhesCargo(this.detalhesCarregados);
    });

    this.form.get('departamentoCodigo')?.valueChanges.subscribe(() => this.sincronizarDetalhesCargo());
    this.form.get('apenasDepartamento')?.valueChanges.subscribe(apenasDepartamento => this.alternarModoPlanejamento(apenasDepartamento));

    this.codigo = this.route.snapshot.paramMap.get('codigo');
    if (this.codigo) {
      this.form.get('codigo')?.disable();
      this.form.get('ano')?.disable();
      this.form.get('departamentoCodigo')?.disable();

      this.service.obterPorCodigo(this.codigo).subscribe({
        next: (plc) => {
          this.form.patchValue(plc);
          this.detalhesCarregados = plc.detalhesCargo ?? [];
          this.sincronizarDetalhesCargo(this.detalhesCarregados);
        },
        error: () => {
          this.notificacaoService.exibirMensagem('Erro ao carregar dados para edicao.', Nivel.Error);
          this.router.navigate(['atron/planejamento-custos']);
        }
      });
    }
  }

  salvar(): void {
    const dadosForm = this.form.getRawValue();
    const resultadoDetalhesCargo = PlanejamentoCustoFormularioState.montarDetalhesCargo(dadosForm);

    const payload = new PlanejamentoCustoRequest(
      dadosForm.codigo,
      dadosForm.descricao,
      Number(dadosForm.ano),
      Number(dadosForm.valorMinimo),
      Number(dadosForm.valorTeto),
      Boolean(dadosForm.apenasDepartamento),
      dadosForm.departamentoCodigo,
      resultadoDetalhesCargo.detalhesCargo
    );

    const request = this.codigo
      ? this.service.atualizar(this.codigo, payload)
      : this.service.gravar(payload);

    request.subscribe({
      next: (resposta: unknown) => {
        this.notificacaoService.exibirMensagens(resposta);
        if (!this.notificacaoService.temMensagemDeErro(resposta)) {
          this.router.navigate(['atron/planejamento-custos']);
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

  private get detalhesCargo(): FormArray {
    return this.form.get('detalhesCargo') as FormArray;
  }

  private sincronizarDetalhesCargo(detalhesExistentes: PlanejamentoCustoCargoResponse[] = []): void {
    if (!this.form) {
      return;
    }

    PlanejamentoCustoFormularioState.sincronizarDetalhesCargo(
      this.fb,
      this.detalhesCargo,
      this.form.get('departamentoCodigo')?.value,
      this.todosCargos,
      detalhesExistentes
    );
  }

  private alternarModoPlanejamento(apenasDepartamento: boolean): void {
    if (apenasDepartamento) {
      return;
    }

    const detalhesAtuais = this.detalhesCargo.getRawValue();
    this.sincronizarDetalhesCargo(detalhesAtuais.length ? detalhesAtuais : this.detalhesCarregados);
  }

}
