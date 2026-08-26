import { MatDialogRef } from '@angular/material/dialog';
import { of } from 'rxjs';
import { CategoriaStatus } from '../../models/categoria.model';
import { CategoriaService } from '../../services/categoria.service';
import {
  CategoriaHistoricoDialogComponent,
  CategoriaHistoricoDialogData
} from './categoria-historico-dialog.component';

describe('CategoriaHistoricoDialogComponent', () => {
  let service: jasmine.SpyObj<CategoriaService>;
  let dialogRef: jasmine.SpyObj<MatDialogRef<CategoriaHistoricoDialogComponent>>;
  let component: CategoriaHistoricoDialogComponent;

  const data: CategoriaHistoricoDialogData = {
    categoria: {
      codigo: 'CAT',
      descricao: 'Categoria',
      status: CategoriaStatus.Removido
    }
  };

  beforeEach(() => {
    service = jasmine.createSpyObj<CategoriaService>('CategoriaService', ['obterAuditoria']);
    dialogRef = jasmine.createSpyObj<MatDialogRef<CategoriaHistoricoDialogComponent>>(
      'MatDialogRef',
      ['close']
    );
    component = new CategoriaHistoricoDialogComponent(data, service, dialogRef);
  });

  it('deve carregar a data de remoção e todos os históricos para paginação local', () => {
    service.obterAuditoria.and.returnValue(of({
      codigoRegistro: 'CAT',
      removidoEm: '2026-08-24T10:00:00',
      historicos: [
        {
          codigoHistorico: 2,
          dataCriacao: '2026-08-24T10:00:00',
          descricao: 'Categoria removida.'
        },
        {
          codigoHistorico: 1,
          dataCriacao: '2026-08-23T10:00:00',
          descricao: 'Categoria criada.'
        }
      ]
    }));

    component.ngOnInit();

    expect(service.obterAuditoria).toHaveBeenCalledWith('CAT');
    expect(component.auditoria?.removidoEm).toBe('2026-08-24T10:00:00');
    expect(component.dataSource.data.length).toBe(2);
    expect(component.carregando).toBeFalse();
  });

  it('deve apresentar estado de erro quando a auditoria não for encontrada', () => {
    service.obterAuditoria.and.returnValue(of(null));

    component.ngOnInit();

    expect(component.erroAoCarregar).toBeTrue();
    expect(component.dataSource.data).toEqual([]);
  });
});
