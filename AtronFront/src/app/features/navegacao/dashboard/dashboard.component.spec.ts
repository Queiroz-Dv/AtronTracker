import { ActivatedRoute, Router } from '@angular/router';
import { DashboardCard } from '../../../core/layout/models/dashboardCard';
import { AcessoService } from '../../../core/services/acesso.service';
import { VisualizacaoService } from '../../../core/services/visualizacao-service';
import { DashboardComponent } from './dashboard.component';

describe('DashboardComponent', () => {
  let router: jasmine.SpyObj<Router>;
  let component: DashboardComponent;

  beforeEach(() => {
    router = jasmine.createSpyObj<Router>('Router', ['navigate', 'navigateByUrl']);
    component = new DashboardComponent(
      router,
      {} as ActivatedRoute,
      {} as AcessoService,
      {} as VisualizacaoService
    );
  });

  it('deve abrir dinamicamente qualquer área recebida pelo card', () => {
    component.navigate(criarCard({ area: 'area-futura' }));

    expect(router.navigate).toHaveBeenCalledWith(
      ['/atron/dashboard'],
      { queryParams: { area: 'area-futura' } }
    );
  });

  it('deve navegar diretamente quando o card representa uma rotina', () => {
    component.navigate(criarCard({ route: '/atron/rotina-futura' }));

    expect(router.navigateByUrl).toHaveBeenCalledWith('/atron/rotina-futura');
  });

  function criarCard(alteracoes: Partial<DashboardCard>): DashboardCard {
    return {
      code: 'CARD',
      title: 'Card',
      icon: 'apps',
      description: 'Descrição',
      accessLabel: 'Acessar',
      route: '/atron/dashboard',
      cols: 1,
      rows: 1,
      ...alteracoes
    };
  }
});
