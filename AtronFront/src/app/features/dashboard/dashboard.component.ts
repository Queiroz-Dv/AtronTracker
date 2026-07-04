import { Component, OnInit } from '@angular/core';
import { DashboardCard } from '../../core/layout/models/dashboardCard';
import { Router, RouterModule } from '@angular/router';
import { MaterialContainerModule } from '../../material-container.module';
import { SharedModule } from '../../shared/modules/shared.module';
import { AcessoService } from '../acesso/login/services/acesso.service';
import { VisualizacaoService } from '../../core/services/visualizacao-service';
import { ModuloItem } from '../../shared/utils/modulo-functions.util';
import { ModuloModel } from '../modulos/interfaces/modulo.interface';

const ORDEM_MODULOS_DASHBOARD = ['DPT', 'CRG', 'USR', 'PLC', 'PERF', 'RPERFUSR', 'TAR', 'TRF'];

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
      this.cardsView = [...modulos]
        .sort((a, b) => this.obterOrdem(a.codigo) - this.obterOrdem(b.codigo))
        .map(m => this.criarCard(m));
    });
  }

  private criarCard(m: ModuloModel): DashboardCard {
    const item = new ModuloItem(m.codigo);
    return {
      code: m.codigo,
      title: item.titulo || m.descricao,
      icon: item.icone || 'default-icon',
      description: item.descricao || 'Descrição não disponível',
      route: item.rota,
      cols: 1,
      rows: 1
    };
  }

  private obterOrdem(codigo: string): number {
    const ordem = ORDEM_MODULOS_DASHBOARD.indexOf(codigo);
    return ordem === -1 ? ORDEM_MODULOS_DASHBOARD.length : ordem;
  }

  navigate(route: string) {
    this.router.navigateByUrl(route);
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
