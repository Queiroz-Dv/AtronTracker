import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, catchError, map, of } from 'rxjs';
import { SessaoInfoService } from '../services/sessaoInfo.service';

@Injectable({ providedIn: 'root' })
export class PerfilInicialGuard implements CanActivate {

  constructor(
    private sessaoService: SessaoInfoService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    return this.sessaoService.obterDadosUsuario().pipe(
      map(dados => {
        const perfis = dados?.perfisDeAcesso || [];
        const semPerfil = perfis.length === 0;
        const tentandoAcessarSetup = state.url.includes('configurar-perfil-inicial');

        // Cenário 1: Usuário sem perfil tentando acessar rotas do sistema -> envia para o setup
        if (semPerfil && !tentandoAcessarSetup) {
          return this.router.createUrlTree(['/configurar-perfil-inicial']);
        }

        // Cenário 2: Usuário que já possui perfil tentando acessar a tela de setup -> envia para a Home
        if (!semPerfil && tentandoAcessarSetup) {
          return this.router.createUrlTree(['/atron/home']);
        }

        // Cenário Padrão: Navegação liberada
        return true;
      }),
      catchError(() => {
        return of(this.router.createUrlTree(['/login']));
      })
    );
  }
}