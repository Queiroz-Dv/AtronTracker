import { Component, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BotaoVoltarComponent } from '../../../../core/layout/botao-voltar/botao-voltar.component';
import { Mensagem, Nivel, NotificacaoService } from '../../../../core/services/notification.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { PlanejamentoCustoRelatorioImpressaoService } from '../../services/planejamento-custo-relatorio-impressao.service';
import { PlanejamentoCustoService } from '../../services/planejamento-custo.service';

@Component({
  selector: 'c-planejamento-custos-relatorio',
  standalone: true,
  imports: [SharedModule, ReactiveFormsModule, BotaoVoltarComponent],
  templateUrl: './planejamento-custos-relatorio.component.html',
  styleUrls: ['../../planejamento-custos.component.css']
})
export class PlanejamentoCustosRelatorioComponent implements OnInit {
  anoControl = new FormControl<number>(new Date().getFullYear());
  carregando = false;
  codigoPlanejamento?: string;

  constructor(
    private service: PlanejamentoCustoService,
    private impressaoService: PlanejamentoCustoRelatorioImpressaoService,
    private route: ActivatedRoute,
    private notificacaoService: NotificacaoService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.codigoPlanejamento = this.route.snapshot.paramMap.get('codigo') ?? undefined;
  }

  imprimir(): void {
    if (this.codigoPlanejamento) {
      this.imprimirPlanejamento(this.codigoPlanejamento);
      return;
    }

    this.carregando = true;
    this.service.obterRelatorioGeralImpressao(Number(this.anoControl.value ?? 0)).subscribe({
      next: htmlRelatorio => {
        this.impressaoService.imprimir(htmlRelatorio);
        this.carregando = false;
      },
      error: erro => {
        this.carregando = false;
        this.tratarErroRelatorio(erro, 'Nao foi possivel carregar o relatorio geral.');
      }
    });
  }

  private imprimirPlanejamento(codigo: string): void {
    this.carregando = true;
    this.service.obterRelatorioPlanejamentoImpressao(codigo).subscribe({
      next: htmlRelatorio => {
        this.impressaoService.imprimir(htmlRelatorio);
        this.carregando = false;
      },
      error: erro => {
        this.carregando = false;
        this.tratarErroRelatorio(erro, 'Nao foi possivel carregar o relatorio do planejamento.');
      }
    });
  }

  get titulo(): string {
    return 'Relatorio de Planejamentos';
  }

  get descricao(): string {
    return '';
  }

  private tratarErroRelatorio(erro: any, mensagemPadrao: string): void {
    const mensagens = erro?.mensagensApi as Mensagem[];
    if (mensagens?.length) {
      this.notificacaoService.exibirMensagens(mensagens);
      return;
    }

    this.notificacaoService.exibirMensagem(mensagemPadrao, Nivel.Error);
  }
}
