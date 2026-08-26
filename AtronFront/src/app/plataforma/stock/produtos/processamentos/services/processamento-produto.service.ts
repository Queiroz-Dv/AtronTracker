import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RotasApi } from '../../../../../shared/models/rotas-api.model';
import { ProcessamentoProduto } from '../models/processamento-produto.model';

@Injectable({ providedIn: 'root' })
export class ProcessamentoProdutoService {
  constructor(private readonly http: HttpClient) { }

  obterTodos(): Observable<ProcessamentoProduto[]> {
    return this.http.get<ProcessamentoProduto[]>(RotasApi.processamentoProdutoEndpoint);
  }

  obterPorId(id: number): Observable<ProcessamentoProduto> {
    return this.http.get<ProcessamentoProduto>(
      `${RotasApi.processamentoProdutoEndpoint}/${id}`
    );
  }
}
