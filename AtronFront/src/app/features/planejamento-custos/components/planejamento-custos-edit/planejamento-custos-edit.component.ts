import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';
import { CargoService } from '../../../cargos/services/cargo.service';
import { PlanejamentoCustoCargoRequest, PlanejamentoCustoRequest } from '../../models/request/planejamento-custo-request.model';
import { PlanejamentoCustoCargoResponse } from '../../models/response/planejamento-custo-response.model';
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
      codigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]],
      descricao: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      ano: [this.anoAtual, [Validators.required, Validators.min(this.anoAtual)]],
      valorMinimo: [0, [Validators.required, Validators.min(0)]],
      valorTeto: [0, [Validators.required, Validators.min(0.01)]],
      apenasDepartamento: [false],
      departamentoCodigo: ['', [Validators.required, Validators.maxLength(10)]],
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
    if (this.form.invalid) {
      this.notificacaoService.exibirMensagem('Formulario invalido. Verifique os campos.', Nivel.Error);
      return;
    }

    const dadosForm = this.form.getRawValue();
    if (Number(dadosForm.valorMinimo) >= Number(dadosForm.valorTeto)) {
      this.notificacaoService.exibirMensagem('O valor minimo deve ser menor que o valor teto.', Nivel.Error);
      return;
    }

    const detalhesCargo = this.montarDetalhesCargo(dadosForm);
    if (!detalhesCargo) {
      return;
    }

    const payload = new PlanejamentoCustoRequest(
      dadosForm.codigo,
      dadosForm.descricao,
      Number(dadosForm.ano),
      Number(dadosForm.valorMinimo),
      Number(dadosForm.valorTeto),
      Boolean(dadosForm.apenasDepartamento),
      dadosForm.departamentoCodigo,
      detalhesCargo
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

    const departamentoCodigo = this.form.get('departamentoCodigo')?.value;
    if (!departamentoCodigo || !this.todosCargos.length) {
      this.detalhesCargo.clear();
      return;
    }

    const detalhesAtuais = this.detalhesCargo.getRawValue();
    const detalhesPorCargo = [...detalhesAtuais, ...detalhesExistentes]
      .reduce((mapa, detalhe) => {
        mapa.set(detalhe.cargoCodigo, detalhe);
        return mapa;
      }, new Map<string, any>());

    const cargosDepartamento = this.todosCargos
      .filter(cargo => cargo.departamentoCodigo === departamentoCodigo)
      .sort((a, b) => a.descricao.localeCompare(b.descricao));

    this.detalhesCargo.clear();

    cargosDepartamento.forEach(cargo => {
      const detalhe = detalhesPorCargo.get(cargo.codigo);
      this.detalhesCargo.push(this.fb.group({
        cargoCodigo: [cargo.codigo],
        cargoDescricao: [cargo.descricao],
        detalhado: [detalhe?.detalhado ?? false],
        valorMinimo: [detalhe?.valorMinimo ?? null],
        valorTeto: [detalhe?.valorTeto ?? null]
      }));
    });
  }

  private alternarModoPlanejamento(apenasDepartamento: boolean): void {
    if (apenasDepartamento) {
      return;
    }

    const detalhesAtuais = this.detalhesCargo.getRawValue();
    this.sincronizarDetalhesCargo(detalhesAtuais.length ? detalhesAtuais : this.detalhesCarregados);
  }

  private montarDetalhesCargo(dadosForm: any): PlanejamentoCustoCargoRequest[] | null {
    if (dadosForm.apenasDepartamento) {
      return [];
    }

    const detalhes = dadosForm.detalhesCargo ?? [];
    if (!detalhes.length) {
      this.notificacaoService.exibirMensagem('O departamento selecionado nao possui cargos para detalhamento.', Nivel.Error);
      return null;
    }

    const detalhados = detalhes.filter((detalhe: any) => detalhe.detalhado);
    if (!detalhados.length) {
      this.notificacaoService.exibirMensagem('Todos os cargos estao como nao detalhados. Use planejamento apenas por departamento.', Nivel.Error);
      return null;
    }

    for (const detalhe of detalhados) {
      const valorMinimo = Number(detalhe.valorMinimo);
      const valorTeto = Number(detalhe.valorTeto);

      if (detalhe.valorMinimo === null || detalhe.valorMinimo === undefined || detalhe.valorMinimo === ''
        || detalhe.valorTeto === null || detalhe.valorTeto === undefined || detalhe.valorTeto === ''
        || Number.isNaN(valorMinimo) || Number.isNaN(valorTeto)) {
        this.notificacaoService.exibirMensagem(`Informe valor minimo e teto para o cargo ${detalhe.cargoCodigo}.`, Nivel.Error);
        return null;
      }

      if (valorMinimo < 0 || valorTeto <= 0 || valorMinimo >= valorTeto) {
        this.notificacaoService.exibirMensagem(`Revise os valores do cargo ${detalhe.cargoCodigo}. O minimo deve ser menor que o teto.`, Nivel.Error);
        return null;
      }
    }

    return detalhes.map((detalhe: any) => new PlanejamentoCustoCargoRequest(
      detalhe.cargoCodigo,
      Boolean(detalhe.detalhado),
      detalhe.detalhado ? Number(detalhe.valorMinimo) : null,
      detalhe.detalhado ? Number(detalhe.valorTeto) : null
    ));
  }
}
