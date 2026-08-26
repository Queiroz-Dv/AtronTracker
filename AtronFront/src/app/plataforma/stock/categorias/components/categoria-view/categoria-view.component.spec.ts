import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { ConfirmacaoExecucaoParams, ConfirmacaoService } from '../../../../../shared/services/confirmacao.service';
import { Categoria, CategoriaStatus } from '../../models/categoria.model';
import { CategoriaService } from '../../services/categoria.service';
import { CategoriaViewComponent } from './categoria-view.component';
import { CategoriaHistoricoDialogComponent } from '../categoria-historico-dialog/categoria-historico-dialog.component';

describe('CategoriaViewComponent', () => {
  let component: CategoriaViewComponent;
  let service: jasmine.SpyObj<CategoriaService>;
  let confirmacaoService: jasmine.SpyObj<ConfirmacaoService>;
  let dialog: jasmine.SpyObj<MatDialog>;

  beforeEach(() => {
    service = jasmine.createSpyObj<CategoriaService>(
      'CategoriaService',
      ['obterTodos', 'deletar']
    );
    confirmacaoService = jasmine.createSpyObj<ConfirmacaoService>(
      'ConfirmacaoService',
      ['confirmarEExecutar']
    );
    dialog = jasmine.createSpyObj<MatDialog>('MatDialog', ['open']);

    component = new CategoriaViewComponent(
      service,
      confirmacaoService,
      dialog,
      jasmine.createSpyObj<Router>('Router', ['navigate'])
    );
  });

  it('deve confirmar a exclusão da categoria selecionada e recarregar a listagem', () => {
    const categoria: Categoria = {
      codigo: 'CAT',
      descricao: 'Categoria',
      status: CategoriaStatus.Ativo
    };
    service.deletar.and.returnValue(of([]));
    const carregar = spyOn(component, 'carregar');

    component.excluir(categoria);

    expect(service.deletar).toHaveBeenCalledWith('CAT');
    expect(confirmacaoService.confirmarEExecutar).toHaveBeenCalled();

    const params = confirmacaoService.confirmarEExecutar.calls.mostRecent()
      .args[0] as ConfirmacaoExecucaoParams;
    expect(params.textoBotaoConfirmar).toBe('Excluir');

    params.onSuccess();
    expect(carregar).toHaveBeenCalled();
  });

  it('deve combinar a busca textual com o filtro de categorias removidas', () => {
    component.dataSource.data = [
      { codigo: 'ATV', descricao: 'Ativa', status: CategoriaStatus.Ativo },
      { codigo: 'REM', descricao: 'Categoria principal', status: CategoriaStatus.Removido },
      { codigo: 'OUT', descricao: 'Outra removida', status: CategoriaStatus.Removido }
    ];

    component.filtrar({ target: { value: 'principal' } } as unknown as Event);
    component.alternarFiltroRemovidas();

    expect(component.somenteRemovidas).toBeTrue();
    expect(component.dataSource.filteredData.map(categoria => categoria.codigo)).toEqual(['REM']);
  });

  it('deve exibir somente categorias ativas e inativas na listagem principal', () => {
    component.dataSource.data = [
      { codigo: 'ATV', descricao: 'Ativa', status: CategoriaStatus.Ativo },
      { codigo: 'INA', descricao: 'Inativa', status: CategoriaStatus.Inativo },
      { codigo: 'REM', descricao: 'Removida', status: CategoriaStatus.Removido }
    ];

    expect(component.somenteRemovidas).toBeFalse();
    expect(component.dataSource.filteredData.map(categoria => categoria.codigo)).toEqual([
      'ATV',
      'INA'
    ]);
  });

  it('deve abrir o histórico da categoria no modal Material', () => {
    const categoria: Categoria = {
      codigo: 'CAT',
      descricao: 'Categoria',
      status: CategoriaStatus.Removido
    };

    component.abrirHistorico(categoria);

    expect(dialog.open).toHaveBeenCalled();
    const [componente, config] = dialog.open.calls.mostRecent().args;
    expect(componente).toBe(CategoriaHistoricoDialogComponent);
    expect(config?.data).toEqual({ categoria });
  });
});
