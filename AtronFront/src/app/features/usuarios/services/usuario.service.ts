import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseService } from '../../../core/services/base-service';
import { UsuarioModel } from '../models/usuario.model';
import { UsuarioResponse } from '../models/response/usuario-response';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService extends BaseService<UsuarioModel> {

  constructor(http: HttpClient) {
    super(http, RotasApi.usuarioEndpoint)
  }

  obterTodosUsuariosInformados(): Observable<UsuarioResponse[]> {
    return this.http.get<UsuarioResponse[]>(RotasApi.usuarioEndpoint);
  }
}