import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AcessoService } from '../../features/acesso/login/services/acesso.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private acessoService: AcessoService) { }

  canActivate(): Observable<boolean> {
    return this.acessoService.modulosAcessiveis$.pipe(
      map(modulos => {
        const isAuthenticated = Array.isArray(modulos) && modulos.length > 0;

        if (!isAuthenticated) {
          console.warn('Usuário sem módulos de acesso válidos. Redirecionando para o login.');
          this.router.navigate(['/login']);
        }
      
        return isAuthenticated;
      }),
      catchError((error) => {
        console.error('Erro ao validar autenticação do usuário no guard:', error);
        this.router.navigate(['/login']);
        return of(false);
      })
    );
  }
}