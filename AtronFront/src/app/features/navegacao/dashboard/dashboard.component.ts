import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import {
  AreaPlataformaConfig,
  LISTA_AREAS_PLATAFORMA
} from '../../../core/config/areas-plataforma.config';
import { DashboardCard } from '../../../core/layout/models/dashboardCard';
import { VisualizacaoService } from '../../../core/services/visualizacao-service';
import { MaterialContainerModule } from '../../../material-container.module';
import { SharedModule } from '../../../shared/modules/shared.module';
import { ModuloItem } from '../../../shared/utils/modulo-functions.util';
import { AcessoService } from '../../../core/services/acesso.service';
import { ModuloModel } from '../modulos/interfaces/modulo.interface';
import { WorkspaceSeletorComponent } from '../workspace-seletor/workspace-seletor.component';

@Component({
  standalone: true,
  selector: 'c-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  imports: [MaterialContainerModule, SharedModule, RouterModule, WorkspaceSeletorComponent]
})
export class DashboardComponent implements OnInit {
  cardsView: DashboardCard[] = [];
  rotinasCardsView: DashboardCard[] = [];
  areaSelecionada: AreaPlataformaConfig | null = null;

  private modulosAcessiveis: ModuloModel[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private acessoService: AcessoService,
    private visualizacaoService: VisualizacaoService
  ) { }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      this.areaSelecionada = this.obterArea(params.get('area'));
      this.atualizarCardsDasRotinas();
    });

    this.acessoService.modulosAcessiveis$.subscribe(modulos => {
      this.modulosAcessiveis = modulos;
      this.cardsView = this.criarCardsAgrupados(modulos);
      this.atualizarCardsDasRotinas();
    });
  }

  get tituloVisualizacao(): string {
    return this.areaSelecionada?.titulo ?? 'Áreas da plataforma';
  }

  get descricaoVisualizacao(): string {
    return this.areaSelecionada?.descricaoRotinas
      ?? 'Acesse as rotinas centrais da operação.';
  }

  navigate(card: DashboardCard): void {
    if (card.disabled) {
      return;
    }

    if (card.area) {
      this.router.navigate(['/atron/dashboard'], {
        queryParams: { area: card.area }
      });
      return;
    }

    this.router.navigateByUrl(card.route);
  }

  voltarParaAreas(): void {
    this.router.navigate(['/atron/dashboard']);
  }

  trocarVisualizacao(): void {
    this.visualizacaoService.setViewMode('menu');
    this.router.navigate(['/atron/home']);
  }

  logout(): void {
    this.acessoService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }

  private obterArea(chave: string | null): AreaPlataformaConfig | null {
    if (!chave) {
      return null;
    }

    return LISTA_AREAS_PLATAFORMA.find(area => area.chave === chave)
      ?? null;
  }

  private atualizarCardsDasRotinas(): void {
    this.rotinasCardsView = this.areaSelecionada
      ? this.criarCardsDasRotinas(
          this.modulosAcessiveis,
          this.areaSelecionada.codigosModulos
        )
      : [];
  }

  private criarCardsAgrupados(modulos: ModuloModel[]): DashboardCard[] {
    const modulosPorCodigo = new Map(modulos.map(modulo => [modulo.codigo, modulo]));

    return LISTA_AREAS_PLATAFORMA.map(area =>
      this.criarCardDaArea(area, modulosPorCodigo)
    );
  }

  private criarCardDaArea(
    area: AreaPlataformaConfig,
    modulosPorCodigo: Map<string, ModuloModel>
  ): DashboardCard {
    const modulosDaArea = area.codigosModulos
      .filter(codigo => modulosPorCodigo.has(codigo))
      .map(codigo => {
        const item = new ModuloItem(codigo);
        const modulo = modulosPorCodigo.get(codigo);

        return {
          titulo: item.titulo === codigo ? modulo?.descricao ?? codigo : item.titulo,
          rota: item.rota
        };
      });

    const rotaInicial = modulosDaArea
      .find(modulo => modulo.rota !== '/atron/default')?.rota
      ?? '/atron/dashboard';
    const areaSemAcesso = modulosDaArea.length === 0;

    return {
      code: area.codigo,
      title: area.titulo,
      icon: area.icone,
      description: area.descricao,
      accessLabel: areaSemAcesso ? 'Sem acesso' : area.rotuloAcesso,
      disabled: area.desabilitado || areaSemAcesso,
      area: area.chave,
      route: rotaInicial,
      cols: 1,
      rows: 1
    };
  }

  private criarCardsDasRotinas(
    modulos: ModuloModel[],
    codigosDaArea: readonly string[]
  ): DashboardCard[] {
    return [...modulos]
      .filter(modulo => codigosDaArea.includes(modulo.codigo))
      .sort((a, b) =>
        codigosDaArea.indexOf(a.codigo) - codigosDaArea.indexOf(b.codigo)
      )
      .map(modulo => this.criarCardDaRotina(modulo));
  }

  private criarCardDaRotina(modulo: ModuloModel): DashboardCard {
    const item = new ModuloItem(modulo.codigo);

    return {
      code: modulo.codigo,
      title: item.titulo || modulo.descricao,
      icon: item.icone || 'default-icon',
      description: item.descricao || 'Descrição não disponível',
      accessLabel: 'Acessar rotina',
      route: item.rota,
      cols: 1,
      rows: 1
    };
  }
}
