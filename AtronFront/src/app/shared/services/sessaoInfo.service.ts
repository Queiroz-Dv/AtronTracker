import { Injectable } from "@angular/core";
import { DadosDoUsuario } from "../../features/acesso/login/models/dados-do-usuario.model";
import { Observable, shareReplay } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { RotasApi } from "../models/rotas-api.model";

export interface UsuarioInfoToken {
  codigoDoUsuario: string;
  token: string;
  expires: Date;
}

export interface AccessTokenInfo {
  token: string;
  expires: Date;
}

@Injectable({ providedIn: "root" })

export class SessaoInfoService {
  private readonly usuarioCodigoStorageKey = 'atron_usuario_codigo';
  private readonly usuarioCodigoStorageKeyLegado = 'usuarioInfo';

  constructor(private http: HttpClient) { }
  
  private usuarioInfoToken: UsuarioInfoToken | null = null;

  private usuarioInfo: Observable<DadosDoUsuario>;

  obterDadosUsuario(): Observable<DadosDoUsuario> {
    return this.usuarioInfo
      ?? (this.usuarioInfo = this.http
        .get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint)
        .pipe(shareReplay({ bufferSize: 1, refCount: true })));
  }

  clearSessionInfo(): void {
    this.usuarioInfo = undefined;
    this.usuarioInfoToken = null;
    localStorage.removeItem(this.usuarioCodigoStorageKey);
    sessionStorage.removeItem(this.usuarioCodigoStorageKey);
    localStorage.removeItem(this.usuarioCodigoStorageKeyLegado);
  }

  setUsuarioInfo(token: string, expires: Date, usuarioCodigo: string): void {
    this.usuarioInfoToken = { codigoDoUsuario: usuarioCodigo, token: token, expires: expires };

    this.definirUsuarioCodigo(usuarioCodigo);
  }

  definirUsuarioCodigo(usuarioCodigo: string): void {
    localStorage.setItem(this.usuarioCodigoStorageKey, usuarioCodigo);
    sessionStorage.removeItem(this.usuarioCodigoStorageKey);
    localStorage.removeItem(this.usuarioCodigoStorageKeyLegado);
  }

  obterUsuarioCodigo(): string | null {
    const usuarioCodigo = localStorage.getItem(this.usuarioCodigoStorageKey);
    if (usuarioCodigo) return usuarioCodigo;

    const usuarioCodigoSessao = sessionStorage.getItem(this.usuarioCodigoStorageKey);
    if (usuarioCodigoSessao) {
      this.definirUsuarioCodigo(usuarioCodigoSessao);
      return usuarioCodigoSessao;
    }

    const usuarioCodigoLegado = localStorage.getItem(this.usuarioCodigoStorageKeyLegado);
    if (!usuarioCodigoLegado) return null;

    this.definirUsuarioCodigo(usuarioCodigoLegado);
    return usuarioCodigoLegado;
  }

  obterAccessToken(): AccessTokenInfo | null {
    if (this.usuarioInfoToken == null) return null;

    const tokenExpiration = new Date(this.usuarioInfoToken.expires);
    if (tokenExpiration.getTime() < Date.now()) {
      return null;
    }

    return {
      token: this.usuarioInfoToken.token,
      expires: this.usuarioInfoToken.expires,
    };
  }

  clearInfo(): void {
    this.usuarioInfo = undefined;
  }
}
