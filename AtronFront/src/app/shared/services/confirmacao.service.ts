import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { filter, switchMap } from 'rxjs/operators';
import { Mensagem, Nivel, NotificacaoService } from '../../core/services/notification.service';
import { ConfirmacaoDialogComponent, ConfirmacaoDialogData } from '../components/confirmacao-dialog/confirmacao-dialog.component';

export interface ConfirmacaoExecucaoParams {
  titulo: string;
  mensagem: string;
  operacao$: Observable<Mensagem[]>;
  onSuccess: () => void;
  textoBotaoConfirmar?: string;
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

    dialogRef.afterClosed().pipe(
      filter(resultado => resultado === true),
      switchMap(() => params.operacao$)
    ).subscribe({
      next: (resposta: Mensagem[]) => {
        this.notificacaoService.exibirMensagens(resposta);
        params.onSuccess();
      },
      error: (erro: any) => {
        if (erro && erro.mensagensApi) {
          this.notificacaoService.exibirMensagens(erro.mensagensApi);
        } else {
          this.notificacaoService.exibirMensagem('Ocorreu um erro ao tentar executar a operação.', Nivel.Error);
        }
      }
    });
  }
}
