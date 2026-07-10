import { Injectable } from '@angular/core';
import { Nivel, NotificacaoService } from '../../../core/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class PlanejamentoCustoRelatorioImpressaoService {
  constructor(private notificacaoService: NotificacaoService) { }

  imprimir(htmlRelatorio: string): void {
    const iframe = document.createElement('iframe');
    iframe.style.position = 'fixed';
    iframe.style.right = '0';
    iframe.style.bottom = '0';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = '0';

    document.body.appendChild(iframe);
    let impressaoDisparada = false;

    const imprimirIframe = () => {
      if (impressaoDisparada) {
        return;
      }

      impressaoDisparada = true;
      const janela = iframe.contentWindow;
      if (!janela) {
        iframe.remove();
        this.notificacaoService.exibirMensagem('Não foi possível preparar o relatório para impressão.', Nivel.Error);
        return;
      }

      janela.focus();
      janela.print();
      setTimeout(() => iframe.remove(), 1000);
    };

    iframe.onload = imprimirIframe;
    iframe.srcdoc = htmlRelatorio;
    setTimeout(imprimirIframe, 250);
  }
}
