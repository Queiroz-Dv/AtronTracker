import { ActivatedRoute, Router } from '@angular/router';
import { VisualizacaoService } from '../../services/visualizacao-service';
import { BotaoVoltarComponent } from './botao-voltar.component';

describe('BotaoVoltarComponent', () => {
  let router: jasmine.SpyObj<Router>;
  let visualizacaoService: jasmine.SpyObj<VisualizacaoService>;

  beforeEach(() => {
    router = jasmine.createSpyObj<Router>('Router', ['navigate', 'navigateByUrl']);
    visualizacaoService = jasmine.createSpyObj<VisualizacaoService>(
      'VisualizacaoService',
      ['getViewMode']
    );
    visualizacaoService.getViewMode.and.returnValue('dashboard');
  });

  it('deve aceitar dinamicamente qualquer área informada pela rota', () => {
    const component = criarComponenteComArea('area-futura');

    component.onVoltar();

    expect(router.navigate).toHaveBeenCalledWith(
      ['/atron/dashboard'],
      { queryParams: { area: 'area-futura' } }
    );
  });

  it('deve voltar para a visão de áreas quando a rota não informa uma área', () => {
    const component = criarComponenteComArea(undefined);

    component.onVoltar();

    expect(router.navigate).toHaveBeenCalledWith(['/atron/dashboard']);
  });

  it('deve continuar voltando para home quando o modo de visualização for menu', () => {
    visualizacaoService.getViewMode.and.returnValue('menu');
    const component = criarComponenteComArea('stock');

    component.onVoltar();

    expect(router.navigate).toHaveBeenCalledWith(['/atron/home']);
  });

  function criarComponenteComArea(area: string | undefined): BotaoVoltarComponent {
    const activatedRoute = {
      snapshot: {
        pathFromRoot: [
          { data: {} },
          { data: area ? { area } : {} }
        ]
      }
    } as ActivatedRoute;

    return new BotaoVoltarComponent(router, visualizacaoService, activatedRoute);
  }
});
