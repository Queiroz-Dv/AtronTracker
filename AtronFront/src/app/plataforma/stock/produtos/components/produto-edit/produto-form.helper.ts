import { HttpErrorResponse } from '@angular/common/http';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Nivel, NotificacaoService } from '../../../../../core/services/notification.service';
import { converterDataParaFormulario, formatarDataParaEnvio } from '../../../../../shared/utils/data-form.utils';
import {
  GeracaoProdutosLoteRequest,
  Produto,
  ProdutoAtualizacao,
  ProdutoCriacao,
  ProdutoFormulario
} from '../../models/produto.model';

export function criarFormularioProduto(fb: FormBuilder): FormGroup {
  return fb.group({
    gerarPorLote: [false],
    codigo: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(25)]],
    quantidade: [null],
    descricao: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(50)]],
    descricaoComplementar: [null],
    dataAquisicao: ['', Validators.required],
    precoUnitario: [null, [Validators.required, Validators.min(0.01)]],
    categoriaCodigos: [[]]
  }, {
    validators: validarComprimentoCodigosGerados
  });
}

export function configurarModoGeracao(form: FormGroup, gerarPorLote: boolean): void {
  const codigo = form.get('codigo');
  const quantidade = form.get('quantidade');

  codigo?.setValidators([
    Validators.required,
    Validators.minLength(gerarPorLote ? 2 : 3),
    Validators.maxLength(gerarPorLote ? 24 : 25)
  ]);

  if (gerarPorLote) {
    quantidade?.setValidators([
      Validators.required,
      Validators.min(1),
      Validators.pattern(/^[1-9]\d*$/)
    ]);
  }

  if (!gerarPorLote) {
    quantidade?.clearValidators();
    quantidade?.setValue(null, { emitEvent: false });
  }

  codigo?.updateValueAndValidity({ emitEvent: false });
  quantidade?.updateValueAndValidity({ emitEvent: false });
  form.updateValueAndValidity({ emitEvent: false });
}

export function preencherFormularioProduto(form: FormGroup, produto: Produto): void {
  form.patchValue({
    codigo: produto.codigo,
    descricao: produto.descricao,
    descricaoComplementar: produto.descricaoComplementar,
    dataAquisicao: converterDataParaFormulario(produto.dataAquisicao),
    precoUnitario: produto.precoUnitario,
    categoriaCodigos: produto.categorias.map(categoria => categoria.codigo)
  });
}

export function montarAtualizacaoProduto(valor: ProdutoCriacao): ProdutoAtualizacao {
  return {
    descricao: valor.descricao,
    descricaoComplementar: valor.descricaoComplementar,
    dataAquisicao: formatarDataParaEnvio(valor.dataAquisicao) ?? '',
    precoUnitario: valor.precoUnitario,
    categoriaCodigos: valor.categoriaCodigos
  };
}

export function montarCriacaoProduto(valor: ProdutoFormulario): ProdutoCriacao {
  return {
    codigo: valor.codigo,
    descricao: valor.descricao,
    descricaoComplementar: valor.descricaoComplementar,
    dataAquisicao: formatarDataParaEnvio(valor.dataAquisicao) ?? '',
    precoUnitario: valor.precoUnitario,
    categoriaCodigos: valor.categoriaCodigos
  };
}

export function montarGeracaoProdutosLote(
  valor: ProdutoFormulario
): GeracaoProdutosLoteRequest {
  return {
    codigoBase: valor.codigo,
    quantidade: Number(valor.quantidade),
    descricao: valor.descricao,
    descricaoComplementar: valor.descricaoComplementar,
    dataAquisicao: formatarDataParaEnvio(valor.dataAquisicao) ?? '',
    precoUnitario: valor.precoUnitario,
    categoriaCodigos: valor.categoriaCodigos
  };
}

function validarComprimentoCodigosGerados(
  formulario: AbstractControl
): ValidationErrors | null {
  if (!formulario.get('gerarPorLote')?.value) {
    return null;
  }

  const codigoBase = String(formulario.get('codigo')?.value ?? '').trim();
  const quantidade = Number(formulario.get('quantidade')?.value);
  if (!codigoBase || !Number.isInteger(quantidade) || quantidade < 1) {
    return null;
  }

  return `${codigoBase}${quantidade}`.length > 25
    ? { codigoGeradoMuitoLongo: true }
    : null;
}

export function exibirErroProduto(notificacao: NotificacaoService, erro: unknown): void {
  const mensagensApi = (erro as { mensagensApi?: unknown })?.mensagensApi;
  if (mensagensApi) {
    notificacao.exibirMensagens(mensagensApi);
  } else if (erro instanceof HttpErrorResponse) {
    notificacao.exibirMensagem(`Erro ${erro.status}: ${erro.statusText}`, Nivel.Error);
  } else {
    notificacao.exibirMensagem('Ocorreu um erro inesperado.', Nivel.Error);
  }
}
