import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError } from "rxjs";
import { SessaoInfoService } from "../services/sessaoInfo.service";
import { Router } from "@angular/router";
import { RotasApi } from "../../shared/models/rotas-api.model";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshSubject = new BehaviorSubject<string | null>(null);

  constructor(private sessaoService: SessaoInfoService, private http: HttpClient, private router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.ehRotaPublica(req)) {
      return next.handle(req);
    }

    if (this.ehRotaRefresh(req)) {
      return next.handle(req.clone({ withCredentials: true }));
    }

    const tokenInfo = this.sessaoService.obterAccessToken();

    if (this.ehRotaLogout(req)) {
      return tokenInfo
        ? next.handle(this.prepararRequisicaoAutenticada(req, tokenInfo.token))
        : next.handle(req.clone({ withCredentials: true }));
    }

    if (!tokenInfo || new Date(tokenInfo.expires).getTime() <= Date.now()) {
      return this.refreshTokenHandle(req, next);
    }

    return next.handle(this.prepararRequisicaoAutenticada(req, tokenInfo.token));
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

  private prepararRequisicaoAutenticada(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      withCredentials: true,
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  private refreshTokenHandle(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);

      return this.http.post<{ value: string, expires: Date, usuarioCodigo: string }>(
        RotasApi.refreshTokenEndpoint,
        {},
        { withCredentials: true }
      ).pipe(
        catchError(error => {
          this.limparSessaoERedirecionar();
          this.refreshSubject.next(null);
          return throwError(() => error);
        }),
        switchMap(response => {
          this.sessaoService.setUsuarioInfo(response.value, response.expires, response.usuarioCodigo);
          this.refreshSubject.next(response.value);
          return next.handle(this.prepararRequisicaoAutenticada(request, response.value));
        }),
        finalize(() => { this.isRefreshing = false; })
      );
    }

    return this.refreshSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => next.handle(this.prepararRequisicaoAutenticada(request, token!)))
    );
  }

  private limparSessaoERedirecionar(): void {
    this.sessaoService.clearSessionInfo();
    this.router.navigate(['/login']);
  }
}
