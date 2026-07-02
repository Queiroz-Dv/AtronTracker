import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RotasApi } from '../../../shared/models/rotas-api.model';
import { BaseService } from '../../../core/services/base-service';
import { UsuarioModel } from '../models/usuario.model';
import { UsuarioResponse } from '../models/response/usuario-response';
import { UsuarioConfiguracoes } from '../models/usuario-configuracoes.model';
import { Mensagem } from '../../../core/services/notification.service';

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

  obterConfiguracoes(): Observable<UsuarioConfiguracoes> {
    return this.http.get<UsuarioConfiguracoes>(`${RotasApi.usuarioEndpoint}/configuracoes`);
  }

  atualizarConfiguracoes(configuracoes: UsuarioConfiguracoes): Observable<Mensagem[]> {
    return this.http.put<Mensagem[]>(`${RotasApi.usuarioEndpoint}/configuracoes`, configuracoes);
  }
}
