import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter, fromEvent, interval, Subscription } from 'rxjs';
import { MaterialContainerModule } from './material-container.module';
import { AcessoService } from './features/acesso/login/services/acesso.service';
import { NotificacaoInterna } from './features/notificacoes/models/notificacao-interna.model';
import { NotificacaoInternaService } from './features/notificacoes/services/notificacao-interna.service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, MaterialContainerModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'atron-manager-webview';
  mostrarNavbarInterna = false;
  notificacoes: NotificacaoInterna[] = [];
  totalNaoLidas = 0;

  private notificacoesPolling?: Subscription;
  private notificacoesSubscription?: Subscription;
  private routerSubscription?: Subscription;
  private visibilitySubscription?: Subscription;

  constructor(
    private router: Router,
    private acessoService: AcessoService,
    private notificacaoInternaService: NotificacaoInternaService
  ) { }

  ngOnInit() {
    this.notificacoesSubscription = this.notificacaoInternaService.notificacoes$.subscribe(notificacoes => {
      this.notificacoes = notificacoes;
      this.totalNaoLidas = notificacoes.filter(notificacao => !notificacao.lida).length;
    });

    this.atualizarNavbarInterna(this.router.url);

    this.routerSubscription = this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe(event => this.atualizarNavbarInterna(event.urlAfterRedirects));

    this.visibilitySubscription = fromEvent(document, 'visibilitychange').subscribe(() => {
      if (!document.hidden && this.mostrarNavbarInterna) {
        this.carregarNotificacoes();
      }
    });
  }

  ngOnDestroy(): void {
    this.notificacoesPolling?.unsubscribe();
    this.notificacoesSubscription?.unsubscribe();
    this.routerSubscription?.unsubscribe();
    this.visibilitySubscription?.unsubscribe();
  }

  abrirTracker() {
    this.router.navigate(['/atron/dashboard'], { queryParams: { produto: 'tracker' } });
  }

  logout() {
    this.acessoService.logout().subscribe(() => {
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

  private atualizarNavbarInterna(url: string) {
    const rotaSemQuery = url.split('?')[0];
    this.mostrarNavbarInterna = rotaSemQuery.startsWith('/atron/')
      && rotaSemQuery !== '/atron/dashboard'
      && rotaSemQuery !== '/atron/home';

    if (this.mostrarNavbarInterna) {
      this.iniciarMonitoramentoNotificacoes();
      return;
    }

    this.pararMonitoramentoNotificacoes();
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
    if (document.hidden) {
      return;
    }

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
}
