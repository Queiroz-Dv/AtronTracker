import { Injectable } from "@angular/core";
import { DadosDoUsuario } from "../../features/acesso/login/models/dados-do-usuario.model";
import { map, Observable, of, shareReplay, tap } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { RotasApi } from "../models/rotas-api.model";

export interface UsuarioInfoToken {
  codigoDoUsuario: string;
  token: string;
  expires: Date;
}

@Injectable({ providedIn: "root" })

export class SessaoInfoService {
  constructor(private http: HttpClient) { }
  
  private usuarioInfoToken: UsuarioInfoToken | null = null;

  private usuarioInfo: Observable<DadosDoUsuario>;

  obterDadosUsuario(): Observable<DadosDoUsuario> {
    return this.usuarioInfo
      ?? (this.usuarioInfo = this.http
        .get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint)
        .pipe(shareReplay({ bufferSize: 1, refCount: true })));
  }

  renovarToken(): Observable<string> {
    return this.http.get<string>(`${RotasApi.sessionInfoEndpoint}`).pipe(tap(token => {
      token;
    }));
  }

  clearSessionInfo(): void {
    this.usuarioInfo = undefined;
    localStorage.removeItem('usuarioInfo');
  }

  setUsuarioInfo(token: string, expires: Date, usuarioCodigo: string): void {
    this.usuarioInfoToken = { codigoDoUsuario: usuarioCodigo, token: token, expires: expires };

    this.setUsuarioCodigoLocalStorage(usuarioCodigo);
  }

  setUsuarioCodigoLocalStorage(usuarioCodigo: string): void {
    localStorage.setItem('usuarioInfo', usuarioCodigo);
  }

  getUsuarioCodigoLocalStorage(): string | null {
    return localStorage.getItem('usuarioInfo');
  }

  getToken(): { token: string, expires: Date } | null {

    if (this.usuarioInfoToken != null) {

      var tokenExpiration = new Date(this.usuarioInfoToken.expires);

      if (tokenExpiration.getTime() < Date.now()) {
        return null        // Token expirado, retornar null
      }

      var info = {
        token: this.usuarioInfoToken.token,
        expires: this.usuarioInfoToken.expires,
      }

      return info;
    }
    return null;
  }


  clearInfo(): void {
    this.usuarioInfo = null;
  }
}
