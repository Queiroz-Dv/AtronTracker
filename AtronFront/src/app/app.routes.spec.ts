import { routes } from './app.routes';
import { HomeComponent } from './features/navegacao/home/home.component';
import { MenuInicioComponent } from './features/navegacao/home/menu-inicio.component';
import { AREAS_PLATAFORMA } from './core/config/areas-plataforma.config';

describe('Rotas autenticadas da plataforma', () => {
  it('mantém o dashboard fora do layout com menu lateral', () => {
    const rotaAtron = routes.find(route => route.path === 'atron');
    const rotaDashboard = rotaAtron?.children?.find(route => route.path === 'dashboard');

    expect(rotaAtron?.component).toBeUndefined();
    expect(rotaDashboard?.component).toBeDefined();
  });

  it('agrupa as telas operacionais dentro do layout lateral', () => {
    const rotaAtron = routes.find(route => route.path === 'atron');
    const rotaLayout = rotaAtron?.children?.find(route => route.component === HomeComponent);
    const caminhosFilhos = rotaLayout?.children?.map(route => route.path);

    expect(caminhosFilhos).toEqual([
      'home',
      'departamentos',
      'cargos',
      'categorias',
      'produtos',
      'planejamento-custos',
      'usuarios',
      'tarefas',
      'notificacoes',
      'perfil-de-acesso'
    ]);
  });

  it('protege categorias com o módulo CAT', () => {
    const rotaAtron = routes.find(route => route.path === 'atron');
    const rotaLayout = rotaAtron?.children?.find(route => route.component === HomeComponent);
    const rotaCategorias = rotaLayout?.children?.find(route => route.path === 'categorias');

    expect(rotaCategorias?.data?.['moduloCodigo']).toBe('CAT');
    expect(rotaCategorias?.data?.['area']).toBe(AREAS_PLATAFORMA.Stock.chave);
  });

  it('protege produtos com o módulo PRD', () => {
    const rotaAtron = routes.find(route => route.path === 'atron');
    const rotaLayout = rotaAtron?.children?.find(route => route.component === HomeComponent);
    const rotaProdutos = rotaLayout?.children?.find(route => route.path === 'produtos');

    expect(rotaProdutos?.data?.['moduloCodigo']).toBe('PRD');
    expect(rotaProdutos?.data?.['area']).toBe(AREAS_PLATAFORMA.Stock.chave);
  });

  it('preserva a rota home como entrada do modo menu', () => {
    const rotaAtron = routes.find(route => route.path === 'atron');
    const rotaLayout = rotaAtron?.children?.find(route => route.component === HomeComponent);
    const rotaHome = rotaLayout?.children?.find(route => route.path === 'home');

    expect(rotaHome?.component).toBe(MenuInicioComponent);
  });
});
