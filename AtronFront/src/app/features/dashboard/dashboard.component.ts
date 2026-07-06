import { Component, OnInit } from '@angular/core';
import { DashboardCard } from '../../core/layout/models/dashboardCard';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MaterialContainerModule } from '../../material-container.module';
import { SharedModule } from '../../shared/modules/shared.module';
import { AcessoService } from '../acesso/login/services/acesso.service';
import { VisualizacaoService } from '../../core/services/visualizacao-service';
import { ModuloItem } from '../../shared/utils/modulo-functions.util';
import { ModuloModel } from '../modulos/interfaces/modulo.interface';

type DashboardModuleGroupConfig = {
  code: string;
  title: string;
  icon: string;
  description: string;
  moduleCodes: string[];
  accessLabel: string;
  disabled?: boolean;
};

const GRUPOS_MODULOS_DASHBOARD: DashboardModuleGroupConfig[] = [
  {
    code: 'ATRON_TRACKER',
    title: 'Atron Tracker',
    icon: 'analytics',
    description: 'Gestão administrativa e operacional.',
    moduleCodes: ['DPT', 'CRG', 'PLC', 'USR', 'PERF', 'RPERFUSR', 'TAR', 'TRF'],
    accessLabel: 'Acessar Tracker'
  },
  {
    code: 'ATRON_STOCK',
    title: 'Atron Stock',
    icon: 'inventory_2',
    description: 'Controle de estoque, patrimônio e rotinas comerciais.',
    moduleCodes: ['EST', 'STK', 'PRD', 'PROD', 'CAT', 'CLI', 'FOR', 'FRN', 'PAT', 'VEN', 'VND'],
    accessLabel: 'Acessar Stock',
    disabled: true
  },
  {
    code: 'ATRON_SALES',
    title: 'Atron Sales',
    icon: 'payments',
    description: 'Gestão financeira e comercial.',
    moduleCodes: ['SAL', 'SALES', 'VEN', 'VND', 'FIN', 'CMP', 'COM'],
    accessLabel: 'Acessar Sales',
    disabled: true
  }
];

@Component({
  standalone: true,
  selector: 'c-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [MaterialContainerModule, SharedModule, RouterModule]
})
export class DashboardComponent implements OnInit {
  cardsView: DashboardCard[] = [];
  trackerCardsView: DashboardCard[] = [];
  visualizacao: 'produtos' | 'tracker' = 'produtos';
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private acessoService: AcessoService,
    private visualizacaoService: VisualizacaoService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.visualizacao = params.get('produto') === 'tracker' ? 'tracker' : 'produtos';
    });

    this.acessoService.modulosAcessiveis$.subscribe(modulos => {
      this.cardsView = this.criarCardsAgrupados(modulos);
      this.trackerCardsView = this.criarCardsDoTracker(modulos);
    });
  }

  private criarCardsAgrupados(modulos: ModuloModel[]): DashboardCard[] {
    const modulosPorCodigo = new Map(modulos.map(modulo => [modulo.codigo, modulo]));
    return GRUPOS_MODULOS_DASHBOARD.map(grupo => this.criarCardDoGrupo(grupo, modulosPorCodigo));
  }

  private criarCardDoGrupo(
    grupo: DashboardModuleGroupConfig,
    modulosPorCodigo: Map<string, ModuloModel>
  ): DashboardCard {
    const modulosDoGrupo = grupo.moduleCodes
      .filter(codigo => modulosPorCodigo.has(codigo))
      .map(codigo => {
        const item = new ModuloItem(codigo);
        const modulo = modulosPorCodigo.get(codigo);

        return {
          titulo: item.titulo === codigo ? modulo?.descricao ?? codigo : item.titulo,
          rota: item.rota
        };
      });

    const rotaInicial = modulosDoGrupo.find(modulo => modulo.rota !== '/atron/default')?.rota ?? '/atron/dashboard';
    const grupoSemAcesso = grupo.code === 'ATRON_TRACKER' && modulosDoGrupo.length === 0;
    const disabled = grupo.disabled || grupoSemAcesso;

    return {
      code: grupo.code,
      title: grupo.title,
      icon: grupo.icon,
      description: grupo.description,
      accessLabel: grupoSemAcesso ? 'Sem acesso' : grupo.accessLabel,
      disabled,
      route: rotaInicial,
      cols: 1,
      rows: 1
    };
  }

  private criarCardsDoTracker(modulos: ModuloModel[]): DashboardCard[] {
    const codigosTracker = GRUPOS_MODULOS_DASHBOARD[0].moduleCodes;

    return [...modulos]
      .filter(modulo => codigosTracker.includes(modulo.codigo))
      .sort((a, b) => codigosTracker.indexOf(a.codigo) - codigosTracker.indexOf(b.codigo))
      .map(modulo => this.criarCardDaRotina(modulo));
  }

  private criarCardDaRotina(m: ModuloModel): DashboardCard {
    const item = new ModuloItem(m.codigo);

    return {
      code: m.codigo,
      title: item.titulo || m.descricao,
      icon: item.icone || 'default-icon',
      description: item.descricao || 'Descrição não disponível',
      accessLabel: 'Acessar rotina',
      route: item.rota,
      cols: 1,
      rows: 1
    };
  }

  navigate(card: DashboardCard) {
    if (card.disabled) {
      return;
    }

    if (card.code === 'ATRON_TRACKER') {
      this.router.navigate(['/atron/dashboard'], { queryParams: { produto: 'tracker' } });
      return;
    }

    this.router.navigateByUrl(card.route);
  }

  voltarParaProdutos() {
    this.router.navigate(['/atron/dashboard']);
  }

  trocarVisualizacao() {
    this.visualizacaoService.setViewMode('menu');
    this.router.navigate(['/atron/home']);
  }

  logout() {
    this.acessoService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
