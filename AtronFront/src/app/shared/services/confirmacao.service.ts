import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import { Mensagem, Nivel, NotificacaoService } from '../../core/services/notification.service';
import { ConfirmacaoDialogComponent, ConfirmacaoDialogData } from '../components/confirmacao-dialog/confirmacao-dialog.component';

/**
 * Interface para os parâmetros do serviço de confirmação.
 */
export interface ConfirmacaoExecucaoParams {
  titulo: string;
  mensagem: string;
  /**
   * O Observable da operação a ser executada (ex: this.service.deletar(id))
   */
  operacao$: Observable<Mensagem[]>; // Ajuste o tipo de retorno se necessário (ex: void)
  /**
   * A função de callback a ser executada em caso de sucesso (ex: () => this.carregar())
   */
  onSuccess: () => void;
  /**
   * O texto do botão de confirmação (opcional)
   */
  textoBotaoConfirmar?: string;
  /**
   * A largura do diálogo (opcional)
   */
  width?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmacaoService {

  constructor(
    private dialog: MatDialog,
    private notificacaoService: NotificacaoService
  ) { }

  /**
   * Abre um diálogo de confirmação e, se confirmado, executa um Observable (operação),
   * tratando as respostas de sucesso e erro.
   *
   * @param params Parâmetros de configuração da confirmação e execução.
   */
  confirmarEExecutar(params: ConfirmacaoExecucaoParams): void {

    const dialogData: ConfirmacaoDialogData = {
      titulo: params.titulo,
      mensagem: params.mensagem,
      textoBotaoConfirmar: params.textoBotaoConfirmar || 'Excluir',
      textoBotaoCancelar: 'Cancelar'
    };

    const dialogRef = this.dialog.open(ConfirmacaoDialogComponent, {
      width: params.width || '500px',
      data: dialogData
    });

    // Aqui está toda a lógica genérica que você replicou
    dialogRef.afterClosed().pipe(
      filter(resultado => resultado === true),
      switchMap(() => params.operacao$) // Executa a operação passada
    ).subscribe({
      next: (resposta: Mensagem[]) => {
        // 1. Exibe a mensagem de sucesso
        this.notificacaoService.exibirMensagens(resposta);
        // 2. Executa o callback de sucesso (ex: recarregar a lista)
        params.onSuccess();
      },
      error: (erro: any) => {
        // 3. Trata o erro de forma centralizada
        if (erro && erro.mensagensApi) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
        } else {
          this.notificacaoService.exibirMensagem('Ocorreu um erro ao tentar executar a operação.', Nivel.Error);
        }
      }
    });
  }
}