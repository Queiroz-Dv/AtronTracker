import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { LoginRequest } from '../../../../shared/models/request/login-request.model';
import { RotasApi } from '../../../../shared/models/rotas-api.model';
import { RegistrarRequest } from '../../../../shared/models/request/registrar-request.model';
import { DadosDoUsuario } from '../models/dados-do-usuario.model';
import { ModuloModel } from '../../../modulos/interfaces/modulo.interface';
import { SessaoInfoService } from '../../../../shared/services/sessaoInfo.service';
import { UserToken } from '../models/userToken';

@Injectable({
  providedIn: 'root'
})
export class AcessoService {

  private sessionInfoSubject = new BehaviorSubject<DadosDoUsuario | null>(null);
  private sessionInfo$: Observable<DadosDoUsuario | null> = this.sessionInfoSubject.asObservable();

  public modulosAcessiveis$: Observable<ModuloModel[]> = this.sessionInfo$.pipe(
    map((info) => info?.perfisDeAcesso?.flatMap((perfil) => perfil.modulos) ?? []),
    shareReplay({ bufferSize: 1, refCount: true }),
  );

  constructor(private http: HttpClient, private sessaoService: SessaoInfoService) { }

  logout(): Observable<boolean> {
    return this.http.get<{ deslogado: boolean }>(RotasApi.desconectarEndpoint, { withCredentials: true }).pipe(
      map(response => {
        this.limparSessaoLocal();
        return response.deslogado;
      }),
      catchError(() => {
        this.limparSessaoLocal();
        return of(false);
      })
    );
  }

  autenticar(login: LoginRequest): Observable<void> {
    return this.http.post<UserToken>(RotasApi.logarEndpoint, login, { withCredentials: true }).pipe(
      switchMap(response => {
        this.sessaoService.setUsuarioInfo(response.token, response.expires, login.codigoDoUsuario);
        return this.carregarSessaoInfo(response.token);
      }),
      map((): void => undefined),
      catchError(error => throwError(() => error))
    );
  }

  restaurarSessaoSeNecessario(): Observable<boolean> {
    if (this.sessionInfoSubject.value) {
      return of(true);
    }

    const tokenAtual = this.sessaoService.obterAccessToken();
    if (tokenAtual) {
      return this.carregarSessaoInfo(tokenAtual.token).pipe(
        map(() => true),
        catchError(() => {
          this.limparSessaoLocal();
          return of(false);
        })
      );
    }

    const usuarioCodigo = this.sessaoService.obterUsuarioCodigo();
    if (!usuarioCodigo) {
      return of(false);
    }

    return this.http.get<UserToken>(RotasApi.refreshTokenEndpoint, { withCredentials: true }).pipe(
      switchMap(token => {
        this.sessaoService.setUsuarioInfo(token.token, token.expires, usuarioCodigo);
        return this.carregarSessaoInfo(token.token);
      }),
      map(() => true),
      catchError(() => {
        this.limparSessaoLocal();
        return of(false);
      })
    );
  }

  usuarioAutenticado(): boolean {
    return this.sessionInfoSubject.value !== null || this.sessaoService.obterUsuarioCodigo() !== null;
  }

  possuiModulo(codigoModulo: string): Observable<boolean> {
    return this.restaurarSessaoSeNecessario().pipe(
      switchMap(sessaoRestaurada => {
        if (!sessaoRestaurada) return of(false);

        return this.modulosAcessiveis$.pipe(
          map(modulos => modulos.some(modulo => modulo.codigo === codigoModulo))
        );
      })
    );
  }

  limparSessaoLocal(): void {
    this.sessaoService.clearSessionInfo();
    this.sessionInfoSubject.next(null);
  }

  private carregarSessaoInfo(token: string): Observable<DadosDoUsuario> {
    if (!token) return throwError(() => new Error('Token de acesso ausente para carregar a sessão.'));

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint, { headers }).pipe(
      tap(info => this.sessionInfoSubject.next(info)),
      catchError(error => {
        this.sessionInfoSubject.next(null);
        return throwError(() => error);
      })
    );
  }

  registrar(dadosDoUsuario: RegistrarRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.registrarEndpoint, dadosDoUsuario).pipe(
      map((response) => response || []),
      catchError((error) => throwError(() => error))
    );
  }

  confirmarEmail(confirmarEmailRequest: ConfirmarEmailRequest): Observable<string> {
    return this.http.post<string>(RotasApi.confirmarEmailEndpoint, confirmarEmailRequest).pipe(
      map(response => response),
      catchError((error) => throwError(() => error))
    );
  }

  solicitarRecuperacaoSenha(request: SolicitarRecuperacaoSenhaRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.recuperarSenhaEndpoint, request).pipe(
      map(response => response || []),
      catchError(error => throwError(() => error))
    );
  }

  trocarSenha(request: RedefinirSenhaRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.trocarSenhaEndpoint, request).pipe(
      map(response => response || []),
      catchError(error => throwError(() => error))
    );
  }
}

export class ConfirmarEmailRequest {
  public usuarioCodigo: string;
  public token: string;
}

export class SolicitarRecuperacaoSenhaRequest {
  public identificador: string;
  public clientUri: string;
}

export class RedefinirSenhaRequest {
  public identificadorTemporario: string;
  public novaSenha: string;
  public repetirSenha: string;
}
