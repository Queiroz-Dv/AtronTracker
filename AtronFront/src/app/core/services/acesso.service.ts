import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, shareReplay, switchMap, tap, throwError } from 'rxjs';
import { SessaoInfoService } from './sessaoInfo.service';
import { ModuloModel } from '../../features/navegacao/modulos/interfaces/modulo.interface';
import { DadosDoUsuario } from '../../plataforma/tracker/acesso/login/models/dados-do-usuario.model';
import { UserToken } from '../../plataforma/tracker/acesso/login/models/userToken';
import { LoginRequest } from '../../shared/models/request/login-request.model';
import { RegistrarRequest } from '../../shared/models/request/registrar-request.model';
import { RegistrarResponse } from '../../shared/models/response/registrar-response.model';
import { RotasApi } from '../../shared/models/rotas-api.model';

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
    private sessaoService: SessaoInfoService
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
        return this.carregarSessaoInfo();
      }),
      map((): void => undefined),
      catchError(error => throwError(() => error))
    );
  }

  restaurarSessaoSeNecessario(): Observable<boolean> {
    if (this.sessionInfoSubject.value) {
      return of(true);
    }

    return this.carregarSessaoInfo().pipe(
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
    return this.carregarSessaoInfo().pipe(
      catchError(error => {
        this.limparSessaoLocal();
        return throwError(() => error);
      })
    );
  }

  limparSessaoLocal(): void {
    this.sessaoService.clearSessionInfo();
    this.sessionInfoSubject.next(null);
  }

  configurarPerfilInicial(payload: any): Observable<any> {
    return this.http.post<any>(RotasApi.configurarPerfilInicialEndpoint, payload);
  }

  private carregarSessaoInfo(): Observable<DadosDoUsuario> {
    this.sessaoService.clearInfo();

    return this.http.get<DadosDoUsuario>(RotasApi.sessionInfoEndpoint).pipe(
      tap(info => {
        this.sessionInfoSubject.next(info);
      }),
      catchError(error => {
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
