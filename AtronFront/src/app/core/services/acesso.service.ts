import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { SessaoInfoService } from './sessaoInfo.service';
import { ModuloModel } from '../../features/navegacao/modulos/interfaces/modulo.interface';
import { DadosDoUsuario } from '../../plataforma/tracker/acesso/login/models/dados-do-usuario.model';
import { UserToken } from '../../plataforma/tracker/acesso/login/models/userToken';
import { LoginRequest } from '../../shared/models/request/login-request.model';
import { RegistrarRequest } from '../../shared/models/request/registrar-request.model';
import { ConviteWorkspaceResponse, RegistrarResponse } from '../../shared/models/response/registrar-response.model';
import { RotasApi } from '../../shared/models/rotas-api.model';
import { WorkspaceContextoService } from './workspace-contexto.service';


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

  constructor(
    private http: HttpClient,
    private sessaoService: SessaoInfoService,
    private workspaceContextoService: WorkspaceContextoService
  ) { }

  logout(): Observable<boolean> {
    return this.http.post(RotasApi.desconectarEndpoint, {}, { withCredentials: true }).pipe(
      map(() => {
        this.limparSessaoLocal();
        return true;
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
        this.sessaoService.setUsuarioInfo(response.value, response.expires, response.usuarioCodigo);
        return this.carregarSessaoInfo(response.value);
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

    return this.http.post<UserToken>(RotasApi.refreshTokenEndpoint, {}, { withCredentials: true }).pipe(
      switchMap(token => {
        this.sessaoService.setUsuarioInfo(token.value, token.expires, token.usuarioCodigo);
        return this.carregarSessaoInfo(token.value);
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

  recarregarSessaoAtual(): Observable<DadosDoUsuario> {
    const tokenAtual = this.sessaoService.obterAccessToken();
    if (tokenAtual) {
      return this.carregarSessaoInfo(tokenAtual.token);
    }

    return this.http.post<UserToken>(RotasApi.refreshTokenEndpoint, {}, { withCredentials: true }).pipe(
      switchMap(token => {
        this.sessaoService.setUsuarioInfo(token.value, token.expires, token.usuarioCodigo);
        return this.carregarSessaoInfo(token.value);
      }),
      catchError(error => {
        this.limparSessaoLocal();
        return throwError(() => error);
      })
    );
  }

  limparSessaoLocal(): void {
    this.sessaoService.clearSessionInfo();
    this.workspaceContextoService.limpar();
    this.sessionInfoSubject.next(null);
  }

  private carregarSessaoInfo(token: string): Observable<DadosDoUsuario> {
    if (!token) return throwError(() => new Error('Token de acesso ausente para carregar a sessão.'));

    this.sessaoService.clearInfo();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint, { headers }).pipe(
      tap(info => {
        this.workspaceContextoService.sincronizarComSessao(info.workspaceAtual);
        this.sessionInfoSubject.next(info);
      }),
      catchError(error => {
        this.workspaceContextoService.limpar();
        this.sessionInfoSubject.next(null);
        return throwError(() => error);
      })
    );
  }

  registrar(dadosDoUsuario: RegistrarRequest): Observable<RegistrarResponse> {
    return this.http.post<RegistrarResponse>(RotasApi.registrarEndpoint, dadosDoUsuario).pipe(
      catchError((error) => throwError(() => error))
    );
  }

  obterConviteWorkspace(identificador: string): Observable<ConviteWorkspaceResponse> {
    return this.http.get<ConviteWorkspaceResponse>(
      `${RotasApi.workspaceEndpoint}/convites/${encodeURIComponent(identificador)}`
    ).pipe(
      catchError((error) => throwError(() => error))
    );
  }

  confirmarEmail(confirmarEmailRequest: ConfirmarEmailRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.confirmarEmailEndpoint, confirmarEmailRequest).pipe(
      map(response => response || []),
      catchError((error) => throwError(() => error))
    );
  }

  solicitarRecuperacaoSenha(request: SolicitarRecuperacaoSenhaRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.recuperarSenhaEndpoint, request).pipe(
      map(response => response || []),
      catchError(error => throwError(() => error))
    );
  }

  reenviarConfirmacaoEmail(request: ReenviarConfirmacaoEmailRequest): Observable<string[]> {
    return this.http.post<string[]>(RotasApi.reenviarConfirmacaoEmailEndpoint, request).pipe(
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
  public identificador: string;
}

export class SolicitarRecuperacaoSenhaRequest {
  public identificador: string;
}

export class ReenviarConfirmacaoEmailRequest {
  public identificador: string;
}

export class RedefinirSenhaRequest {
  public identificadorTemporario: string;
  public novaSenha: string;
  public repetirSenha: string;
}
