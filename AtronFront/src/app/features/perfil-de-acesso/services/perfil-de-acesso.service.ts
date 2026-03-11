import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from '../../../core/services/base-service';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { PerfilDeAcessoModel } from '../interfaces/perfil-de-acesso.interface';
import { catchError, Observable } from 'rxjs';
import { PerfilUsuarioModel } from '../../relacionamento-perfil-usuario/models/perfil-usuario.model';

@Injectable({ providedIn: 'root' })

export class PerfilDeAcessoService extends BaseService<PerfilDeAcessoModel> {

  constructor(http: HttpClient) { super(http, RotasApi.perfilDeAcessoEndpoint) }

  obterRelacionamentoPorCodigoDoPerfil(codigo: string): Observable<PerfilUsuarioModel> {
    return this.http.get<PerfilUsuarioModel>(`${RotasApi.obter_relacionamentoPerfilUsuarioEndpoint}/${codigo}`)
      .pipe(catchError(this.handleError));
  }

  gravarRelacionamento(relacionamento: any): Observable<PerfilUsuarioModel> {
    return this.http.post<any>(RotasApi.gravar_relacionamentoPerfilUsuarioEndpoint, relacionamento).pipe(catchError(this.handleError))
  }
}