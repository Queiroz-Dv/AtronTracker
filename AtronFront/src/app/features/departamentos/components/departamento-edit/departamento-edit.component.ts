import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DepartamentosService } from '../../services/departamentos.service';
import { DepartamentoFormComponent } from "../departamento-form/departamento-form.component";
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Departamento } from '../../models/departamento.model';
import { HttpErrorResponse } from '@angular/common/http';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';

@Component({
  standalone: true,
  selector: 'c-departamento-edit',
  imports: [DepartamentoFormComponent, ReactiveFormsModule, SharedModule],
  templateUrl: './departamento-edit.component.html',
})

export class DepartamentoEditComponent implements OnInit {
  form!: FormGroup;
  codigo?: string | number;

  private notificacaoService = inject(NotificacaoService);

  constructor(
    private fb: FormBuilder,
    private service: DepartamentosService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(10)]],
      descricao: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]]
    });

    this.codigo = this.route.snapshot.paramMap.get('codigo');
    if (this.codigo) {
      this.form.get('codigo')?.disable();
      this.service.obterPorCodigo(this.codigo).subscribe({
        next: (dep) => this.form.patchValue(dep),
        error: () => {
          this.notificacaoService.exibirMensagem("Erro ao carregar dados para edição.", Nivel.Error);
          this.router.navigate(['atron/departamentos']);
        }
      });
    }
  }

  salvar() {
    if (this.form.invalid) {
      this.notificacaoService.exibirMensagem("Formulário inválido. Verifique os campos.", Nivel.Error);
      return;
    }

    const dadosForm = this.form.getRawValue();

    const codigoParaSalvar = this.codigo ? this.codigo : dadosForm.codigo;
    const departamentoPayload = new Departamento(codigoParaSalvar, dadosForm.descricao);

    const request = this.codigo
      ? this.service.atualizar(this.codigo, departamentoPayload)
      : this.service.gravar(departamentoPayload);

    request.subscribe({
      next: (resposta: Mensagem[]) => {
        this.notificacaoService.exibirMensagens(resposta);
        if (!resposta.some(m => m.nivel === Nivel.Error)) {
          this.router.navigate(['atron/departamentos']);
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