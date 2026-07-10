import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError } from "rxjs";
import { SessaoInfoService } from "../../shared/services/sessaoInfo.service";
import { RotasApi } from "../../shared/models/rotas-api.model";
import { Router } from "@angular/router";

export enum HeaderInfo {
  refreshToken = 'XUSRRTK',
  usuarioCodigo = 'XUSRCD'
}

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private sessaoService: SessaoInfoService, private http: HttpClient, private router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const usuarioCodigo = this.sessaoService.obterUsuarioCodigo();

    if (this.ehRotaPublica(req)) {
      return next.handle(req);
    }

    if (this.ehRotaLogout(req)) {
      return next.handle(this.prepararRequisicaoDeSessao(req, usuarioCodigo, 'false'));
    }

    if (this.ehRotaRefresh(req)) {
      return next.handle(this.prepararRequisicaoDeSessao(req, usuarioCodigo, 'true'));
    }

    const tokenInfo = this.sessaoService.obterAccessToken();
    if (!tokenInfo) {
      return this.refreshTokenHandle(req, next, usuarioCodigo);
    }

    const tokenExpiration = new Date(tokenInfo.expires);
    if (tokenExpiration.getTime() <= Date.now()) {
      return this.refreshTokenHandle(req, next, usuarioCodigo);
    }

    return next.handle(this.prepararRequisicaoAutenticada(req, tokenInfo.token, usuarioCodigo));
  }

  private ehRotaPublica(request: HttpRequest<any>): boolean {
    const rotasPublicas = [
      RotasApi.logarEndpoint,
      RotasApi.registrarEndpoint,
      RotasApi.reenviarConfirmacaoEmailEndpoint,
      RotasApi.recuperarSenhaEndpoint,
      RotasApi.trocarSenhaEndpoint,
      RotasApi.confirmarEmailEndpoint
    ];

    return rotasPublicas.some(rota => request.url.startsWith(rota));
  }

  private ehRotaLogout(request: HttpRequest<any>): boolean {
    return request.url.startsWith(RotasApi.desconectarEndpoint);
  }

  private ehRotaRefresh(request: HttpRequest<any>): boolean {
    return request.url.startsWith(RotasApi.refreshTokenEndpoint);
  }

  private prepararRequisicaoDeSessao(
    request: HttpRequest<any>,
    usuarioCodigo: string | null,
    refreshTokenHeader: 'true' | 'false'
  ): HttpRequest<any> {
    return request.clone({
      withCredentials: true,
      setHeaders: {
        [HeaderInfo.refreshToken]: refreshTokenHeader,
        [HeaderInfo.usuarioCodigo]: usuarioCodigo ?? ''
      }
    });
  }

  private prepararRequisicaoAutenticada(
    request: HttpRequest<any>,
    token: string,
    usuarioCodigo: string | null
  ): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        [HeaderInfo.usuarioCodigo]: usuarioCodigo ?? ''
      }
    });
  }

  private refreshTokenHandle(
    request: HttpRequest<any>,
    next: HttpHandler,
    usuarioCodigo: string | null
  ): Observable<HttpEvent<any>> {
    if (!usuarioCodigo) {
      this.limparSessaoERedirecionar();
      return throwError(() => new Error('Código do usuário ausente para renovar a sessão.'));
    }

    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);

      return this.http.get<{ token: string, expires: Date }>(RotasApi.refreshTokenEndpoint, { withCredentials: true }).pipe(
        switchMap(response => {
          this.sessaoService.setUsuarioInfo(response.token, response.expires, usuarioCodigo);
          this.refreshSubject.next(response.token);
          return next.handle(this.prepararRequisicaoAutenticada(request, response.token, usuarioCodigo));
        }),
        catchError(error => {
          this.limparSessaoERedirecionar();
          this.refreshSubject.next(null);
          return throwError(() => error);
        }),
        finalize(() => { this.isRefreshing = false; })
      );
    }

    return this.refreshSubject.pipe(
      filter(t => t != null),
      take(1),
      switchMap(newToken => next.handle(this.prepararRequisicaoAutenticada(request, newToken!, usuarioCodigo)))
    );
  }

  private limparSessaoERedirecionar(): void {
    this.sessaoService.clearSessionInfo();
    this.router.navigate(['/login']);
  }
}
