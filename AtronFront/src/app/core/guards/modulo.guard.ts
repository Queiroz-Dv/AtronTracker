import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { catchError, map, Observable, of, take } from 'rxjs';
import { AcessoService } from '../../features/acesso/login/services/acesso.service';

@Injectable({ providedIn: 'root' })
export class ModuloGuard implements CanActivate {

  constructor(private router: Router, private acessoService: AcessoService) { }

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const codigoModulo = route.data?.['moduloCodigo'] as string | undefined;

    if (!codigoModulo) {
      return of(true);
    }

    return this.acessoService.possuiModulo(codigoModulo).pipe(
      take(1),
      map(possuiModulo => {
        if (!possuiModulo) {
          this.router.navigate(['/atron/dashboard']);
        }

        return possuiModulo;
      }),
      catchError(() => {
        this.router.navigate(['/atron/dashboard']);
        return of(false);
      })
    );
  }
}
