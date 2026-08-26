import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import { CategoriaService } from './categoria.service';

describe('CategoriaService', () => {
  let service: CategoriaService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CategoriaService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CategoriaService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve consultar categorias inativas no endpoint específico', () => {
    service.obterInativas().subscribe(categorias => expect(categorias).toEqual([]));

    const request = httpTesting.expectOne(`${RotasApi.categoriaEndpoint}/inativas`);
    expect(request.request.method).toBe('GET');
    request.flush([]);
  });

  it('deve alterar o status pelo endpoint de ativação', () => {
    service.alterarStatus('CAT', true).subscribe(mensagens => expect(mensagens).toEqual([]));

    const request = httpTesting.expectOne(
      `${RotasApi.categoriaEndpoint}/ativar-inativar/CAT/true`
    );
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual({});
    request.flush([]);
  });

  it('deve excluir uma categoria pelo código', () => {
    service.deletar('CAT').subscribe(mensagens => expect(mensagens).toEqual([]));

    const request = httpTesting.expectOne(`${RotasApi.categoriaEndpoint}/CAT`);
    expect(request.request.method).toBe('DELETE');
    request.flush([]);
  });

  it('deve consultar a auditoria completa da categoria sem paginação no backend', () => {
    service.obterAuditoria('CAT').subscribe(auditoria => {
      expect(auditoria?.codigoRegistro).toBe('CAT');
      expect(auditoria?.historicos.length).toBe(1);
    });

    const request = httpTesting.expectOne(`${RotasApi.auditoriaEndpoint}/CAT/Categoria`);
    expect(request.request.method).toBe('GET');
    expect(request.request.params.keys().length).toBe(0);
    request.flush({
      codigoRegistro: 'CAT',
      removidoEm: '2026-08-24T10:00:00',
      historicos: [
        {
          codigoHistorico: 1,
          dataCriacao: '2026-08-24T10:00:00',
          descricao: 'Categoria removida.'
        }
      ]
    });
  });
});
