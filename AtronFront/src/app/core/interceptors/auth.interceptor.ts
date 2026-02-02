import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, filter, finalize, Observable, switchMap, take } from "rxjs";
import { SessaoInfoService } from "../../shared/services/sessaoInfo.service";
import { RotasApi } from "../../shared/models/rotas-api.model";

export enum HeaderInfo {
  refreshToken = 'XUSRRTK',
  usuarioCodigo = 'XUSRCD'
}

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  private usuarioCodigo: string | null = null;

  constructor(private sessaoService: SessaoInfoService, private http: HttpClient) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    this.usuarioCodigo = this.sessaoService.getUsuarioCodigoLocalStorage();

    if (req.url.startsWith(RotasApi.logarEndpoint) || req.url.startsWith(RotasApi.registrarEndpoint)) {
      this.sessaoService.clearSessionInfo();
      return next.handle(req);
    }

    if (req.url.startsWith(RotasApi.desconectarEndpoint)) {
      req = req.clone({
        setHeaders: { [HeaderInfo.refreshToken]: 'false', [HeaderInfo.usuarioCodigo]: this.usuarioCodigo }
      });

      return next.handle(req); // Não fazer refresh se a própria requisição é pro logout
    }

    if (req.url.endsWith(RotasApi.refreshTokenEndpoint)) {
      req = req.clone({
        setHeaders: { [HeaderInfo.refreshToken]: 'true', [HeaderInfo.usuarioCodigo]: this.usuarioCodigo }
      });
      return next.handle(req); // Não fazer refresh se a própria requisição é pro refresh
    }

    var tokenInfo = this.sessaoService.getToken();

    console.log(tokenInfo);

    if (tokenInfo) {
      const tokenExpiration = new Date(tokenInfo.expires);
      if (tokenExpiration.getTime() <= Date.now()) {
        // Token expirado, precisa renovar
        return this.refreshTokenHandle(req, next);
      } else {
        // Token válido
        const authReq = this.addTokenHeader(req, tokenInfo.token);
        return next.handle(authReq);
      }
    } else {
      
      if (!req.url.endsWith(RotasApi.confirmarEmailEndpoint)) {
        return this.refreshTokenHandle(req, next);
      }else{
        return next.handle(req);
      }
    }
  }

  private addTokenHeader(request: HttpRequest<any>, token: string) {
    return request.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  private refreshTokenHandle(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshSubject.next(null);

      // Chama o endpoint de refresh
      return this.http.get<{ token: string, expires: Date }>(RotasApi.refreshTokenEndpoint, { withCredentials: true }).pipe(
        switchMap(response => {
          const newToken = response;
          this.sessaoService.setUsuarioInfo(newToken.token, newToken.expires, this.usuarioCodigo);
          this.refreshSubject.next(newToken.token);
          return next.handle(this.addTokenHeader(request, newToken.token));
        }),
        finalize(() => { this.isRefreshing = false; })
      );
    } else {
      // Caso já esteja refrescando, espera o novo token
      return this.refreshSubject.pipe(
        filter(t => t != null),
        take(1),
        switchMap(newToken => next.handle(this.addTokenHeader(request, newToken!)))
      );
    }
  }
}