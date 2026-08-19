import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import { TarefaService } from './tarefa.service';

describe('TarefaService histórico', () => {
  let service: TarefaService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TarefaService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(TarefaService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve solicitar a coleção completa sem parâmetros de paginação', () => {
    service.obterHistorico(42).subscribe(movimentacoes => {
      expect(movimentacoes).toEqual([]);
    });

    const request = httpTesting.expectOne(
      `${RotasApi.tarefaEndpoint}/42/Movimentacoes`
    );
    expect(request.request.method).toBe('GET');
    expect(request.request.params.keys()).toEqual([]);
    request.flush([]);
  });
});
