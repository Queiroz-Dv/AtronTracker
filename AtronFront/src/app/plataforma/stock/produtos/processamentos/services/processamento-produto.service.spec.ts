import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RotasApi } from '../../../../../shared/models/rotas-api.model';
import {
  ProcessamentoProduto,
  ProcessamentoProdutoStatus
} from '../models/processamento-produto.model';
import { ProcessamentoProdutoService } from './processamento-produto.service';

describe('ProcessamentoProdutoService', () => {
  let service: ProcessamentoProdutoService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProcessamentoProdutoService, provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ProcessamentoProdutoService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('consulta somente a coleção exposta para o usuário autenticado', () => {
    service.obterTodos().subscribe(itens => expect(itens[0].id).toBe(37));

    const chamada = httpTesting.expectOne(RotasApi.processamentoProdutoEndpoint);
    expect(chamada.request.method).toBe('GET');
    chamada.flush([criarResposta()]);
  });

  it('consulta o detalhe pelo identificador', () => {
    service.obterPorId(37).subscribe(item => expect(item.status).toBe(
      ProcessamentoProdutoStatus.Concluido
    ));

    const chamada = httpTesting.expectOne(`${RotasApi.processamentoProdutoEndpoint}/37`);
    expect(chamada.request.method).toBe('GET');
    chamada.flush(criarResposta());
  });

  function criarResposta(): ProcessamentoProduto {
    return {
      id: 37, status: ProcessamentoProdutoStatus.Concluido, codigoBase: 'NOTE',
      quantidadeSolicitada: 100, quantidadeProcessada: 100,
      descricao: 'Notebook patrimonial', descricaoComplementar: null,
      dataAquisicao: '2026-08-26', precoUnitario: 3500, categoriaCodigos: ['INF'],
      loteProdutoId: 4, loteProdutoCodigo: 'NOTE_2026', erro: null
    };
  }
});
