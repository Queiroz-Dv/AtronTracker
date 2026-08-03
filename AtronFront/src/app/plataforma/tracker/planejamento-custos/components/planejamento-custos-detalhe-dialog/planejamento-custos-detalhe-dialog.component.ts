import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import { PlanejamentoCustoResponse } from '../../models/response/planejamento-custo-response.model';

@Component({
  selector: 'c-planejamento-custos-detalhe-dialog',
  standalone: true,
  imports: [SharedModule, MatDialogModule],
  templateUrl: './planejamento-custos-detalhe-dialog.component.html',
  styleUrls: ['../../planejamento-custos.component.css']
})
export class PlanejamentoCustosDetalheDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public planejamento: PlanejamentoCustoResponse,
    private dialogRef: MatDialogRef<PlanejamentoCustosDetalheDialogComponent>
  ) { }

  get cargosDetalhados() {
    return this.planejamento.detalhesCargo?.filter(detalhe => detalhe.detalhado) ?? [];
  }

  fechar(): void {
    this.dialogRef.close();
  }
}
