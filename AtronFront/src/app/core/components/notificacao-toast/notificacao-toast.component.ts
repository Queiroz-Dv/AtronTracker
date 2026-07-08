import { Component, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { SharedModule } from '../../../shared/modules/shared.module';
import { Nivel } from '../../services/notification.service';

export interface NotificacaoToastData {
  mensagem: string;
  nivel: Nivel;
}

@Component({
  selector: 'c-notificacao-toast',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './notificacao-toast.component.html',
  styleUrl: './notificacao-toast.component.css'
})
export class NotificacaoToastComponent {
  constructor(
    @Inject(MAT_SNACK_BAR_DATA) public data: NotificacaoToastData,
    private snackBarRef: MatSnackBarRef<NotificacaoToastComponent>
  ) { }

  fechar(): void {
    this.snackBarRef.dismiss();
  }

  obterIcone(): string {
    switch (this.data.nivel) {
      case Nivel.Sucesso: return 'check_circle';
      case Nivel.Error: return 'error';
      case Nivel.Aviso: return 'warning';
      default: return 'info';
    }
  }

  obterTitulo(): string {
    switch (this.data.nivel) {
      case Nivel.Sucesso: return 'Operacao concluida';
      case Nivel.Error: return 'Nao foi possivel concluir';
      case Nivel.Aviso: return 'Atencao';
      default: return 'Mensagem do sistema';
    }
  }
}
