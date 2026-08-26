import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BaseGenericService, RespostaApi } from '../../../../core/services/base-service';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import {
  GeracaoProdutosLoteRequest,
  Produto,
  ProdutoAtualizacao,
  ProdutoAuditoria,
  ProdutoCriacao,
  SolicitacaoGeracaoProdutosLoteResponse
} from '../models/produto.model';

@Injectable({ providedIn: 'root' })
export class ProdutoService extends BaseGenericService<ProdutoCriacao, Produto> {
  constructor(http: HttpClient) {
    super(http, RotasApi.produtoEndpoint);
  }

  criar(request: ProdutoCriacao): Observable<RespostaApi> {
    return this.http.post<RespostaApi>(this.endpoint, request)
      .pipe(catchError(this.tratarErro));
  }

  criarLote(
    request: GeracaoProdutosLoteRequest
  ): Observable<SolicitacaoGeracaoProdutosLoteResponse> {
    return this.http.post<SolicitacaoGeracaoProdutosLoteResponse>(
      `${this.endpoint}/lotes`,
      request
    ).pipe(catchError(this.tratarErro));
  }

  atualizarProduto(codigo: string, request: ProdutoAtualizacao): Observable<RespostaApi> {
    return this.http.put<RespostaApi>(`${this.endpoint}/${encodeURIComponent(codigo)}`, request)
      .pipe(catchError(this.tratarErro));
  }

  obterAuditoria(codigo: string): Observable<ProdutoAuditoria | null> {
    return this.http.get<ProdutoAuditoria | null>(
      `${RotasApi.auditoriaEndpoint}/${encodeURIComponent(codigo)}/Produto`
    ).pipe(catchError(this.tratarErro));
  }
}
