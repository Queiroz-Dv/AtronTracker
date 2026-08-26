import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, finalize } from 'rxjs';
import { RespostaApi } from '../../../../../core/services/base-service';
import { Nivel, NotificacaoService } from '../../../../../core/services/notification.service';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { Categoria, CategoriaStatus } from '../../../categorias/models/categoria.model';
import { CategoriaService } from '../../../categorias/services/categoria.service';
import {
  ProdutoFormulario,
  obterDescricaoProdutoStatus
} from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';
import { ProdutoFormComponent } from '../produto-form/produto-form.component';
import {
  configurarModoGeracao,
  criarFormularioProduto,
  exibirErroProduto,
  montarCriacaoProduto,
  montarGeracaoProdutosLote,
  montarAtualizacaoProduto,
  preencherFormularioProduto
} from './produto-form.helper';

@Component({
  selector: 'c-produto-edit',
  standalone: true,
  imports: [ReactiveFormsModule, SharedModule, ProdutoFormComponent],
  templateUrl: './produto-edit.component.html'
})
export class ProdutoEditComponent implements OnInit {
  form!: FormGroup;
  codigo?: string;
  categoriasAtivas: Categoria[] = [];
  statusDescricao = 'Ativo';
  dataEfetivaBaixa: string | null = null;
  salvando = false;

  private readonly notificacao = inject(NotificacaoService);
  private readonly destroyRef = inject(DestroyRef);

  constructor(
    private readonly fb: FormBuilder,
    private readonly produtoService: ProdutoService,
    private readonly categoriaService: CategoriaService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) { }

  ngOnInit(): void {
    this.form = criarFormularioProduto(this.fb);
    this.form.get('gerarPorLote')?.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(gerarPorLote => configurarModoGeracao(this.form, !!gerarPorLote));

    this.categoriaService.obterTodos().subscribe(categorias => {
      this.categoriasAtivas = categorias.filter(
        categoria => categoria.status === CategoriaStatus.Ativo
      );
    });

    this.codigo = this.route.snapshot.paramMap.get('codigo') ?? undefined;
    if (this.codigo) this.carregarProduto(this.codigo);
  }

  salvar(): void {
    if (this.form.invalid || this.salvando) {
      this.form.markAllAsTouched();
      this.notificacao.exibirMensagem('Formulário inválido. Verifique os campos.', Nivel.Error);
      return;
    }

    const valor = this.form.getRawValue() as ProdutoFormulario;

    this.salvando = true;
    if (this.codigo) {
      this.executarGravacao(this.produtoService.atualizarProduto(
        this.codigo,
        montarAtualizacaoProduto(valor)
      ));
      return;
    }

    if (valor.gerarPorLote) {
      this.solicitarGeracaoLote(valor);
      return;
    }

    this.executarGravacao(this.produtoService.criar(montarCriacaoProduto(valor)));
  }

  get gerarPorLote(): boolean {
    return !!this.form?.get('gerarPorLote')?.value;
  }

  get rotuloAcao(): string {
    if (this.salvando)
      return this.gerarPorLote ? 'Solicitando...' : 'Salvando...';

    return this.gerarPorLote ? 'Solicitar geração' : 'Salvar';
  }

  private executarGravacao(operacao: Observable<RespostaApi>): void {
    operacao.pipe(finalize(() => this.salvando = false)).subscribe({
      next: mensagens => {
        this.notificacao.exibirMensagens(mensagens);
        if (!this.notificacao.temMensagemDeErro(mensagens)) {
          this.voltar();
        }
      },
      error: erro => exibirErroProduto(this.notificacao, erro)
    });
  }

  private solicitarGeracaoLote(valor: ProdutoFormulario): void {
    this.produtoService.criarLote(montarGeracaoProdutosLote(valor))
      .pipe(finalize(() => this.salvando = false))
      .subscribe({
        next: resposta => {
          this.notificacao.exibirMensagem(
            `Processamento #${resposta.processamentoId} solicitado com sucesso.`,
            Nivel.Sucesso
          );
          this.router.navigate([
            '/atron/produtos/processamentos',
            resposta.processamentoId
          ]);
        },
        error: erro => exibirErroProduto(this.notificacao, erro)
      });
  }

  voltar(): void {
    this.router.navigate(['/atron/produtos']);
  }

  private carregarProduto(codigo: string): void {
    this.form.get('codigo')?.disable();
    this.produtoService.obterPorCodigo(codigo).subscribe({
      next: produto => {
        preencherFormularioProduto(this.form, produto);
        this.statusDescricao = obterDescricaoProdutoStatus(produto.status);
        this.dataEfetivaBaixa = produto.dataEfetivaBaixa;
      },
      error: erro => {
        exibirErroProduto(this.notificacao, erro);
        this.voltar();
      }
    });
  }

}
