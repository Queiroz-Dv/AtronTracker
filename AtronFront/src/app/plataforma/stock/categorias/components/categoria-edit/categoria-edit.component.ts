import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { Mensagem, Nivel, NotificacaoService } from '../../../../../core/services/notification.service';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { Categoria, CategoriaStatus } from '../../models/categoria.model';
import { CategoriaService } from '../../services/categoria.service';
import { CategoriaFormComponent } from '../categoria-form/categoria-form.component';

@Component({
  selector: 'c-categoria-edit',
  standalone: true,
  imports: [ReactiveFormsModule, SharedModule, CategoriaFormComponent],
  templateUrl: './categoria-edit.component.html'
})
export class CategoriaEditComponent implements OnInit {
  form!: FormGroup;
  codigo?: string;
  salvando = false;

  private readonly notificacaoService = inject(NotificacaoService);

  constructor(
    private readonly fb: FormBuilder,
    private readonly service: CategoriaService,
    private readonly route: ActivatedRoute,
    public readonly router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', [Validators.required, Validators.maxLength(25)]],
      descricao: ['', [Validators.required, Validators.maxLength(50)]],
      status: [CategoriaStatus.Ativo, [Validators.required]]
    });

    this.codigo = this.route.snapshot.paramMap.get('codigo') ?? undefined;
    if (!this.codigo) {
      return;
    }

    this.form.get('codigo')?.disable();
    this.service.obterPorCodigo(this.codigo).subscribe({
      next: categoria => this.form.patchValue(categoria),
      error: () => {
        this.notificacaoService.exibirMensagem(
          'Erro ao carregar os dados da categoria.',
          Nivel.Error
        );
        this.voltar();
      }
    });
  }

  salvar(): void {
    if (this.form.invalid || this.salvando) {
      this.form.markAllAsTouched();
      this.notificacaoService.exibirMensagem(
        'Formulário inválido. Verifique os campos.',
        Nivel.Error
      );
      return;
    }

    const categoria = this.form.getRawValue() as Categoria;
    const operacao = this.codigo
      ? this.service.atualizar(this.codigo, categoria)
      : this.service.gravar(categoria);

    this.salvando = true;
    operacao.pipe(finalize(() => this.salvando = false)).subscribe({
      next: (mensagens: Mensagem[]) => {
        this.notificacaoService.exibirMensagens(mensagens);
        if (!this.notificacaoService.temMensagemDeErro(mensagens)) {
          this.voltar();
        }
      },
      error: erro => this.exibirErro(erro)
    });
  }

  voltar(): void {
    this.router.navigate(['/atron/categorias']);
  }

  private exibirErro(erro: unknown): void {
    const mensagensApi = (erro as { mensagensApi?: unknown })?.mensagensApi;
    if (mensagensApi) {
      this.notificacaoService.exibirMensagens(mensagensApi);
      return;
    }

    if (erro instanceof HttpErrorResponse) {
      this.notificacaoService.exibirMensagem(
        `Erro ${erro.status}: ${erro.statusText}`,
        Nivel.Error,
        6000
      );
      return;
    }

    this.notificacaoService.exibirMensagem(
      'Ocorreu um erro inesperado.',
      Nivel.Error,
      6000
    );
  }
}
