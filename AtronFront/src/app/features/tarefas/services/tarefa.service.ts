import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseService } from '../../../core/services/base-service';
import { TarefaRequest } from '../models/request/tarefa-request.model';
import { TarefaResponse } from '../models/response/tarefa-response.model';
import { EstadoTarefa } from '../models/estadoTarefa.model';
import { SolicitacaoObtencaoTarefaResponse } from '../models/response/solicitacao-obtencao-tarefa-response.model';

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

  obterMeuQuadro(): Observable<TarefaResponse[]> {
    return this.http.get<TarefaResponse[]>(`${RotasApi.tarefaEndpoint}/MeuQuadro`);
  }

  obterEquipe(): Observable<TarefaResponse[]> {
    return this.http.get<TarefaResponse[]>(`${RotasApi.tarefaEndpoint}/Equipe`);
  }

  obterDisponiveis(): Observable<TarefaResponse[]> {
    return this.http.get<TarefaResponse[]>(`${RotasApi.tarefaEndpoint}/Disponiveis`);
  }

  obterSolicitacoes(): Observable<SolicitacaoObtencaoTarefaResponse[]> {
    return this.http.get<SolicitacaoObtencaoTarefaResponse[]>(`${RotasApi.tarefaEndpoint}/Solicitacoes`);
  }

  assumir(id: number): Observable<TarefaResponse> {
    return this.http.post<TarefaResponse>(`${RotasApi.tarefaEndpoint}/${id}/Assumir`, {});
  }

  solicitarObtencao(id: number): Observable<SolicitacaoObtencaoTarefaResponse> {
    return this.http.post<SolicitacaoObtencaoTarefaResponse>(`${RotasApi.tarefaEndpoint}/${id}/SolicitarObtencao`, {});
  }

  aprovarSolicitacao(id: number): Observable<SolicitacaoObtencaoTarefaResponse> {
    return this.http.post<SolicitacaoObtencaoTarefaResponse>(`${RotasApi.tarefaEndpoint}/Solicitacoes/${id}/Aprovar`, {});
  }

  recusarSolicitacao(id: number): Observable<SolicitacaoObtencaoTarefaResponse> {
    return this.http.post<SolicitacaoObtencaoTarefaResponse>(`${RotasApi.tarefaEndpoint}/Solicitacoes/${id}/Recusar`, {});
  }

  obterTarefaPorIdService(id: number): Observable<TarefaResponse> {
    return this.http.get<TarefaResponse>(`${RotasApi.tarefaEndpoint}/${id}`);
  }

  obterEstados(): Observable<EstadoTarefa[]> {
    return this.http.get<EstadoTarefa[]>(RotasApi.tarefaEstadosEndpoint);
  }
}
