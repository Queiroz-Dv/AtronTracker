import { Injectable } from '@angular/core';
import { CanActivateChild, Router, UrlTree } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { EmpresaContextoService } from '../services/empresa-contexto.service';

@Injectable({ providedIn: 'root' })
export class EmpresaGuard implements CanActivateChild {
  constructor(private empresa: EmpresaContextoService, private router: Router) { }

  canActivateChild(): Observable<boolean | UrlTree> {
    return this.empresa.obter().pipe(
      map(contexto => contexto?.acessoPermitido === true
        ? true
        : this.router.createUrlTree(['/empresa/acesso'])),
      catchError(() => of(this.router.createUrlTree(['/empresa/acesso'])))
    );
  }
}
