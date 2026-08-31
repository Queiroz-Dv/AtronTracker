import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { map } from 'rxjs/operators';
import { Router, RouterModule } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { VisualizacaoService } from '../../../core/services/visualizacao-service';
import { SharedModule } from '../../../shared/modules/shared.module';
import { AcessoService } from '../../../core/services/acesso.service';
import { MaterialContainerModule } from '../../../material-container.module';
import { fromEvent, interval, Subscription } from 'rxjs';
import {
  NotificacaoInterna,
  normalizarTextoNotificacao
} from '../../../plataforma/notificacoes/models/notificacao-interna.model';
import { NotificacaoInternaService } from '../../../plataforma/notificacoes/services/notificacao-interna.service';
import { WorkspaceSeletorComponent } from '../workspace-seletor/workspace-seletor.component';
import { ModuloModel } from '../modulos/interfaces/modulo.interface';
import { ModuloItem } from '../../../shared/utils/modulo-functions.util';


@Component({
  selector: 'atron-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    SharedModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MaterialContainerModule,
    RouterModule,
    WorkspaceSeletorComponent
  ]
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly breakpointObserver = inject(BreakpointObserver);
  readonly modoMenu = this.visualizacaoService.getViewMode() === 'menu';
  readonly menuAberto = signal(
    this.modoMenu && !this.breakpointObserver.isMatched('(max-width: 900px)')
  );

  notificacoes: NotificacaoInterna[] = [];
  totalNaoLidas = 0;

  private notificacoesPolling?: Subscription;
  private notificacoesSubscription?: Subscription;
  private visibilitySubscription?: Subscription;
  private modulosSubscription?: Subscription;

  readonly modulosView = signal<ModuloItem[]>([]);
  readonly isCompact = toSignal(
    this.breakpointObserver.observe('(max-width: 900px)')
      .pipe(map(result => result.matches)),
    { initialValue: this.breakpointObserver.isMatched('(max-width: 900px)') }
  );

  constructor(
    private router: Router,
    private visualizacaoService: VisualizacaoService,
    private authService: AcessoService,
    private notificacaoInternaService: NotificacaoInternaService
  ) { }

  ngOnInit(): void {
    this.notificacoesSubscription = this.notificacaoInternaService.notificacoes$.subscribe(notificacoes => {
      this.notificacoes = notificacoes;
      this.totalNaoLidas = notificacoes.filter(notificacao => !notificacao.lida).length;
    });
    this.modulosSubscription = this.authService.modulosAcessiveis$.subscribe(modulos => {
      this.modulosView.set(this.criarModulosMenu(modulos));
    });

    this.iniciarMonitoramentoNotificacoes();
    this.visibilitySubscription = fromEvent(document, 'visibilitychange').subscribe(() => {
      if (!document.hidden) {
        this.carregarNotificacoes();
      }
    });
  }

  ngOnDestroy(): void {
    this.pararMonitoramentoNotificacoes();
    this.notificacoesSubscription?.unsubscribe();
    this.visibilitySubscription?.unsubscribe();
    this.modulosSubscription?.unsubscribe();
  }

  trocarVisualizacao() {
    this.visualizacaoService.setViewMode('dashboard');
    this.router.navigate(['/atron/dashboard']);
  }

  alternarMenu(): void {
    this.menuAberto.update(aberto => !aberto);
  }

  fecharMenuCompacto(): void {
    if (this.isCompact()) {
      this.menuAberto.set(false);
    }
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.pararMonitoramentoNotificacoes();
      this.notificacaoInternaService.limparCache();
      this.router.navigate(['/login']);
    });
  }

  abrirCentralNotificacoes(): void {
    this.router.navigate(['/atron/notificacoes']);
  }

  abrirNotificacao(notificacao: NotificacaoInterna): void {
    const navegar = () => this.navegarParaDestino(notificacao.urlDestino);

    if (notificacao.lida) {
      navegar();
      return;
    }

    this.notificacaoInternaService.marcarComoLida(notificacao.id).subscribe({
      next: navegar,
      error: navegar
    });
  }

  marcarNotificacaoComoLida(notificacao: NotificacaoInterna): void {
    if (notificacao.lida) return;
    this.notificacaoInternaService.marcarComoLida(notificacao.id).subscribe();
  }

  excluirNotificacao(notificacao: NotificacaoInterna): void {
    this.notificacaoInternaService.excluir(notificacao.id).subscribe();
  }

  formatarTextoNotificacao(texto: string): string {
    return normalizarTextoNotificacao(texto);
  }

  private iniciarMonitoramentoNotificacoes(): void {
    if (this.notificacoesPolling) return;

    this.carregarNotificacoes();
    this.notificacoesPolling = interval(60000).subscribe(() => this.carregarNotificacoes());
  }

  private pararMonitoramentoNotificacoes(): void {
    this.notificacoesPolling?.unsubscribe();
    this.notificacoesPolling = undefined;
  }

  private carregarNotificacoes(): void {
    if (document.hidden) return;

    this.notificacaoInternaService.carregar().subscribe({
      error: () => undefined
    });
  }

  private navegarParaDestino(urlDestino: string): void {
    if (!urlDestino) {
      this.abrirCentralNotificacoes();
      return;
    }

    this.router.navigateByUrl(urlDestino.startsWith('/') ? urlDestino : `/${urlDestino}`);
  }

  private criarModulosMenu(modulos: ModuloModel[]): ModuloItem[] {
    return modulos.map(modulo => new ModuloItem(modulo.codigo));
  }
}
