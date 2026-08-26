import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import {
  GeracaoProdutosLoteRequest,
  ProdutoAtualizacao,
  ProdutoCriacao
} from '../models/produto.model';
import { ProdutoService } from './produto.service';

describe('ProdutoService', () => {
  let service: ProdutoService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProdutoService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ProdutoService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve criar um produto pelo contrato patrimonial', () => {
    const request = criarRequest();
    service.criar(request).subscribe(mensagens => {
      expect(mensagens[0].descricao).toBe('Produto salvo com sucesso.');
    });

    const chamada = httpTesting.expectOne(RotasApi.produtoEndpoint);
    expect(chamada.request.method).toBe('POST');
    expect(chamada.request.body).toEqual(request);
    chamada.flush([{ descricao: 'Produto salvo com sucesso.', nivel: 'Sucesso' }]);
  });

  it('deve atualizar sem enviar código, status ou data de baixa', () => {
    const request: ProdutoAtualizacao = {
      descricao: 'Notebook atualizado',
      descricaoComplementar: null,
      dataAquisicao: '2026-08-24',
      precoUnitario: 3600,
      categoriaCodigos: []
    };
    service.atualizarProduto('MTG30', request).subscribe(mensagens => {
      expect(mensagens[0].descricao).toBe('Registro MTG30 atualizado com sucesso.');
    });

    const chamada = httpTesting.expectOne(`${RotasApi.produtoEndpoint}/MTG30`);
    expect(chamada.request.method).toBe('PUT');
    expect(chamada.request.body).toEqual(request);
    chamada.flush([
      { descricao: 'Registro MTG30 atualizado com sucesso.', nivel: 'Sucesso' }
    ]);
  });

  it('deve solicitar a geração em lote e receber o identificador do processamento', () => {
    const request: GeracaoProdutosLoteRequest = {
      codigoBase: 'NOTE',
      quantidade: 100,
      descricao: 'Notebook patrimonial',
      descricaoComplementar: null,
      dataAquisicao: '2026-08-26',
      precoUnitario: 3500,
      categoriaCodigos: ['INF']
    };

    service.criarLote(request).subscribe(resposta => {
      expect(resposta.processamentoId).toBe(37);
    });

    const chamada = httpTesting.expectOne(`${RotasApi.produtoEndpoint}/lotes`);
    expect(chamada.request.method).toBe('POST');
    expect(chamada.request.body).toEqual(request);
    chamada.flush({ processamentoId: 37, status: 1 });
  });

  it('deve consultar a auditoria usando o contexto Produto', () => {
    service.obterAuditoria('MTG 30').subscribe();

    const chamada = httpTesting.expectOne(
      `${RotasApi.auditoriaEndpoint}/MTG%2030/Produto`
    );
    expect(chamada.request.method).toBe('GET');
    chamada.flush(null);
  });

  function criarRequest(): ProdutoCriacao {
    return {
      codigo: 'MTG30',
      descricao: 'Notebook patrimonial',
      descricaoComplementar: null,
      dataAquisicao: '2026-08-24',
      precoUnitario: 3500,
      categoriaCodigos: []
    };
  }
});
