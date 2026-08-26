import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BaseGenericService, RespostaApi } from '../../../../core/services/base-service';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import { Categoria, CategoriaAuditoria } from '../models/categoria.model';

@Injectable({ providedIn: 'root' })
export class CategoriaService extends BaseGenericService<Categoria> {
  constructor(http: HttpClient) {
    super(http, RotasApi.categoriaEndpoint);
  }

  obterInativas(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${this.endpoint}/inativas`)
      .pipe(catchError(this.tratarErro));
  }

  alterarStatus(codigo: string, ativar: boolean): Observable<RespostaApi> {
    return this.http.put<RespostaApi>(
      `${this.endpoint}/ativar-inativar/${codigo}/${ativar}`,
      {}
    ).pipe(catchError(this.tratarErro));
  }

  obterAuditoria(codigo: string): Observable<CategoriaAuditoria | null> {
    const codigoRegistro = encodeURIComponent(codigo);
    return this.http.get<CategoriaAuditoria | null>(
      `${RotasApi.auditoriaEndpoint}/${codigoRegistro}/Categoria`
    ).pipe(catchError(this.tratarErro));
  }
}
