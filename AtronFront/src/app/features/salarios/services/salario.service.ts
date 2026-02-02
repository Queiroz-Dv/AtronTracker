import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseService } from '../../../core/services/base-service';
import { SalarioRequest } from '../models/request/salario-request';
import { Observable } from 'rxjs';
import { SalarioResponse } from '../models/response/salario-response';

@Injectable({ providedIn: 'root' })
export class SalarioService extends BaseService<SalarioRequest> {

  constructor(http: HttpClient) {
    super(http, RotasApi.salarioEndpoint);
  }

  listar(): Observable<SalarioResponse[]> { return this.http.get<SalarioResponse[]>(RotasApi.salarioEndpoint); }

  obterSalarioPorId(id: number): Observable<SalarioResponse> { return this.http.get<SalarioResponse>(`${RotasApi.salarioEndpoint}/${id}`); }
}