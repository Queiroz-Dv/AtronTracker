import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { finalize } from 'rxjs';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import {
  Produto,
  ProdutoAuditoria,
  ProdutoHistoricoAuditoria
} from '../../models/produto.model';
import { ProdutoService } from '../../services/produto.service';

export interface ProdutoHistoricoDialogData {
  produto: Produto;
}

@Component({
  selector: 'c-produto-historico-dialog',
  standalone: true,
  imports: [SharedModule, MatDialogModule],
  templateUrl: './produto-historico-dialog.component.html',
  styleUrl: './produto-historico-dialog.component.css'
})
export class ProdutoHistoricoDialogComponent implements OnInit {
  readonly colunas = ['dataCriacao', 'descricao'];
  readonly dataSource = new MatTableDataSource<ProdutoHistoricoAuditoria>([]);
  auditoria: ProdutoAuditoria | null = null;
  carregando = false;
  erroAoCarregar = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public readonly data: ProdutoHistoricoDialogData,
    private readonly service: ProdutoService,
    private readonly dialogRef: MatDialogRef<ProdutoHistoricoDialogComponent>
  ) { }

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.erroAoCarregar = false;
    this.service.obterAuditoria(this.data.produto.codigo)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: auditoria => {
          this.auditoria = auditoria;
          this.dataSource.data = auditoria?.historicos ?? [];
          this.erroAoCarregar = !auditoria;
        },
        error: () => {
          this.auditoria = null;
          this.dataSource.data = [];
          this.erroAoCarregar = true;
        }
      });
  }

  fechar(): void {
    this.dialogRef.close();
  }
}
