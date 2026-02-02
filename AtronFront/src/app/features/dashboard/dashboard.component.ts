import { Component, inject, OnInit } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import { DashboardCard } from '../../core/layout/models/dashboardCard';
import { Router, RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { MaterialContainerModule } from '../../material-container.module';
import { SharedModule } from '../../shared/modules/shared.module';
import { AcessoService } from '../acesso/login/services/acesso.service';
import { VisualizacaoService } from '../../core/services/visualizacao-service';
import { ModuloItem } from '../../shared/utils/modulo-functions.util';
import { ModuloModel } from '../modulos/interfaces/modulo.interface';

@Component({
  standalone: true,
  selector: 'c-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [MaterialContainerModule, SharedModule, RouterModule]
})
export class DashboardComponent implements OnInit {
  cardsView: DashboardCard[] = [];

  constructor(
    private router: Router,
    private acessoService: AcessoService,
    private visualizacaoService: VisualizacaoService) { }

  ngOnInit() {
    this.acessoService.modulosAcessiveis$.subscribe(modulos => {
      this.cardsView = modulos.map(m => this.criarCard(m));
    });
  }

  private criarCard(m: ModuloModel): DashboardCard {
    const item = new ModuloItem(m.codigo);
    return {
      code: m.codigo,
      title: m.descricao,
      icon: item.icone || 'default-icon',
      description: item.descricao || 'Descrição não disponível',
      route: item.rota,
      cols: 1,
      rows: 1
    };
  }

  navigate(route: string) {
    this.router.navigateByUrl(route);
  }

  private breakpointObserver = inject(BreakpointObserver);

  setCardsForView() {
    this.cards.subscribe(cards => {
      this.cardsView = cards;
    });
  }

  cards: Observable<DashboardCard[]> = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => this.getCards(matches))
  );

  private getCards(isHandset: boolean): DashboardCard[] {
    return this.cardsView;
  }

  trocarVisualizacao() {
    this.visualizacaoService.setViewMode('menu');
    this.router.navigate(['/atron/home']);
  }

  authToken: string = 'authToken'; // Define the authToken property
  usuarioTempData: string = 'usuarioTempData'; // Define the usuarioTempData property

  logout() {
    this.acessoService.logout().subscribe((retorno) => {     
        // Aqui você obtém o valor de retorno do método logout
        console.log('Retorno do logout:', retorno);
        localStorage.removeItem(this.authToken);
        localStorage.removeItem(this.usuarioTempData);
        this.router.navigate(['/login']);
    });
  }
}