import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseService } from '../../../core/services/base-service';
import { TarefaRequest } from '../models/request/tarefa-request.model';
import { TarefaResponse } from '../models/response/tarefa-response.model';

@Injectable({
  providedIn: 'root'
})
export class TarefaService extends BaseService<TarefaRequest> {

  constructor(http: HttpClient) {
    super(http, RotasApi.tarefaEndpoint)
  }

  obterTodasTarefasRelacionadas(): Observable<TarefaResponse[]> {
    return this.http.get<TarefaResponse[]>(RotasApi.tarefaEndpoint);
  }

  obterTarefaPorIdService(id: number): Observable<TarefaResponse> {
    return this.http.get<TarefaResponse>(`${RotasApi.tarefaEndpoint}/${id}`);
  }
}