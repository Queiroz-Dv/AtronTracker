import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { BotaoVoltarComponent } from '../../../../core/layout/botao-voltar/botao-voltar.component';
import { NotificacaoInterna } from '../../models/notificacao-interna.model';
import { NotificacaoInternaService } from '../../services/notificacao-interna.service';

type FiltroNotificacao = 'todas' | 'naoLidas';

@Component({
  selector: 'c-notificacao-interna-view',
  standalone: true,
  imports: [SharedModule, BotaoVoltarComponent],
  templateUrl: './notificacao-interna-view.component.html',
  styleUrl: './notificacao-interna-view.component.css'
})
export class NotificacaoInternaViewComponent implements OnInit {
  notificacoes: NotificacaoInterna[] = [];
  notificacoesFiltradas: NotificacaoInterna[] = [];
  filtroAtual: FiltroNotificacao = 'todas';
  carregando = false;
  marcandoTodasComoLidas = false;

  constructor(
    private service: NotificacaoInternaService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.service.notificacoes$.subscribe(notificacoes => {
      this.notificacoes = notificacoes;
      this.atualizarFiltro();
    });
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.service.carregar().subscribe({
      next: () => this.carregando = false,
      error: () => this.carregando = false
    });
  }

  abrir(notificacao: NotificacaoInterna): void {
    const navegar = () => this.navegarParaDestino(notificacao.urlDestino);

    if (notificacao.lida) {
      navegar();
      return;
    }

    this.service.marcarComoLida(notificacao.id).subscribe(() => navegar());
  }

  marcarComoLida(notificacao: NotificacaoInterna): void {
    if (notificacao.lida) return;
    this.service.marcarComoLida(notificacao.id).subscribe();
  }

  marcarTodasComoLidas(): void {
    if (!this.obterTotalNaoLidas() || this.marcandoTodasComoLidas) return;

    this.marcandoTodasComoLidas = true;
    this.service.marcarTodasComoLidas().subscribe({
      next: () => this.marcandoTodasComoLidas = false,
      error: () => this.marcandoTodasComoLidas = false
    });
  }

  alterarFiltro(filtro: FiltroNotificacao): void {
    this.filtroAtual = filtro;
    this.atualizarFiltro();
  }

  obterTotalNaoLidas(): number {
    return this.notificacoes.filter(notificacao => !notificacao.lida).length;
  }

  obterTotalLidas(): number {
    return this.notificacoes.filter(notificacao => notificacao.lida).length;
  }

  private atualizarFiltro(): void {
    this.notificacoesFiltradas = this.filtroAtual === 'naoLidas'
      ? this.notificacoes.filter(notificacao => !notificacao.lida)
      : this.notificacoes;
  }

  private navegarParaDestino(urlDestino: string): void {
    if (!urlDestino) return;
    this.router.navigateByUrl(urlDestino.startsWith('/') ? urlDestino : `/${urlDestino}`);
  }
}
