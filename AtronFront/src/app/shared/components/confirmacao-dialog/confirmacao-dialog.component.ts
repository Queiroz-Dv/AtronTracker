import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

export interface ConfirmacaoDialogData {
  titulo: string;
  mensagem: string;
  textoBotaoConfirmar?: string;
  textoBotaoCancelar?: string;
}

@Component({
  selector: 'app-confirmacao-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  templateUrl: './confirmacao-dialog.component.html',
})
export class ConfirmacaoDialogComponent {

  // Recebe os dados via injeção de dependência
  constructor(
    public dialogRef: MatDialogRef<ConfirmacaoDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmacaoDialogData
  ) { }

  // Fecha o diálogo retornando 'false' (cancelou)
  onCancelarClick(): void {
    this.dialogRef.close(false);
  }

  // Fecha o diálogo retornando 'true' (confirmou)
  onConfirmarClick(): void {
    this.dialogRef.close(true);
  }
}