import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BaseGenericService } from '../../../core/services/base-service';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { PlanejamentoCustoRequest } from '../models/request/planejamento-custo-request.model';
import { PlanejamentoCustoRelatorioGeralResponse } from '../models/response/planejamento-custo-relatorio-response.model';
import { PlanejamentoCustoResponse } from '../models/response/planejamento-custo-response.model';

@Injectable({ providedIn: 'root' })
export class PlanejamentoCustoService extends BaseGenericService<PlanejamentoCustoRequest, PlanejamentoCustoResponse> {
  constructor(http: HttpClient) {
    super(http, RotasApi.planejamentoCustoEndpoint);
  }

  obterRelatorioGeral(ano: number): Observable<PlanejamentoCustoRelatorioGeralResponse> {
    return this.http
      .get<PlanejamentoCustoRelatorioGeralResponse>(`${RotasApi.planejamentoCustoEndpoint}/relatorio-geral?ano=${ano}`)
      .pipe(catchError(this.tratarErro));
  }

  obterRelatorioPorPlanejamento(codigo: string): Observable<PlanejamentoCustoRelatorioGeralResponse> {
    return this.http
      .get<PlanejamentoCustoRelatorioGeralResponse>(`${RotasApi.planejamentoCustoEndpoint}/relatorio/${codigo}`)
      .pipe(catchError(this.tratarErro));
  }
}
