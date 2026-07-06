import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AcessoService } from '../../features/acesso/login/services/acesso.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private acessoService: AcessoService) { }

  canActivate(): Observable<boolean> {
    return this.acessoService.restaurarSessaoSeNecessario().pipe(
      map(sessaoRestaurada => {
        if (!sessaoRestaurada) {
          this.router.navigate(['/login']);
        }

        return sessaoRestaurada;
      }),
      catchError((error) => {
        console.error('Erro ao validar autenticação do usuário no guard:', error);
        this.router.navigate(['/login']);
        return of(false);
      })
    );
  }
}
