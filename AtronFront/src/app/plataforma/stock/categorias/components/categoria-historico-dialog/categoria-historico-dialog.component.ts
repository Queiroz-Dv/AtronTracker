import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { finalize } from 'rxjs';
import { SharedModule } from '../../../../../shared/modules/shared.module';
import {
  Categoria,
  CategoriaAuditoria,
  CategoriaHistoricoAuditoria
} from '../../models/categoria.model';
import { CategoriaService } from '../../services/categoria.service';

export interface CategoriaHistoricoDialogData {
  categoria: Categoria;
}

@Component({
  selector: 'c-categoria-historico-dialog',
  standalone: true,
  imports: [SharedModule, MatDialogModule],
  templateUrl: './categoria-historico-dialog.component.html',
  styleUrl: './categoria-historico-dialog.component.css'
})
export class CategoriaHistoricoDialogComponent implements OnInit {
  readonly colunas = ['dataCriacao', 'descricao'];
  readonly dataSource = new MatTableDataSource<CategoriaHistoricoAuditoria>([]);

  auditoria: CategoriaAuditoria | null = null;
  carregando = false;
  erroAoCarregar = false;

  @ViewChild(MatPaginator)
  set paginator(paginator: MatPaginator | undefined) {
    this.dataSource.paginator = paginator ?? null;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) public readonly data: CategoriaHistoricoDialogData,
    private readonly service: CategoriaService,
    private readonly dialogRef: MatDialogRef<CategoriaHistoricoDialogComponent>
  ) { }

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando = true;
    this.erroAoCarregar = false;

    this.service.obterAuditoria(this.data.categoria.codigo)
      .pipe(finalize(() => this.carregando = false))
      .subscribe({
        next: auditoria => {
          if (!auditoria) {
            this.registrarErro();
            return;
          }

          this.auditoria = auditoria;
          this.dataSource.data = auditoria.historicos ?? [];
          this.dataSource.paginator?.firstPage();
        },
        error: () => this.registrarErro()
      });
  }

  fechar(): void {
    this.dialogRef.close();
  }

  private registrarErro(): void {
    this.auditoria = null;
    this.dataSource.data = [];
    this.erroAoCarregar = true;
  }
}
