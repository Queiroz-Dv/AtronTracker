import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable()
export class EmpresaInterceptor implements HttpInterceptor {
  constructor(private router: Router) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error.status === 403 && error.error?.codigo === 'EMPRESA_ACESSO_BLOQUEADO') {
          void this.router.navigate(['/empresa/acesso']);
        }
        return throwError(() => error);
      })
    );
  }
}
