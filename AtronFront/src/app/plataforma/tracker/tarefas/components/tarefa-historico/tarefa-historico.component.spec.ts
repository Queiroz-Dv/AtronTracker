import { By } from '@angular/platform-browser';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { TestBed } from '@angular/core/testing';
import { MatPaginator } from '@angular/material/paginator';
import { of } from 'rxjs';
import { TarefaMovimentacao } from '../../models/tarefa-movimentacao.model';
import { TarefaService } from '../../services/tarefa.service';
import { TarefaHistoricoComponent } from './tarefa-historico.component';

describe('TarefaHistoricoComponent', () => {
  it('deve carregar uma vez e paginar localmente o grid', async () => {
    const movimentacoes = criarMovimentacoes(6);
    const tarefaService = jasmine.createSpyObj<TarefaService>('TarefaService', [
      'obterHistorico'
    ]);
    tarefaService.obterHistorico.and.returnValue(of(movimentacoes));
    await TestBed.configureTestingModule({
      imports: [TarefaHistoricoComponent],
      providers: [
        { provide: TarefaService, useValue: tarefaService },
        provideNoopAnimations()
      ]
    }).compileComponents();
    const fixture = TestBed.createComponent(TarefaHistoricoComponent);
    fixture.componentRef.setInput('tarefaId', 42);
    fixture.detectChanges();

    fixture.componentInstance.alternar();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(tarefaService.obterHistorico).toHaveBeenCalledOnceWith(42);
    expect(fixture.componentInstance.dataSource.data).toEqual(movimentacoes);
    expect(fixture.nativeElement.querySelectorAll('tr.mat-mdc-row').length).toBe(5);

    const paginator = fixture.debugElement.query(By.directive(MatPaginator))
      .componentInstance as MatPaginator;
    expect(fixture.componentInstance.dataSource.paginator).toBe(paginator);
    paginator.nextPage();
    fixture.detectChanges();

    expect(paginator.pageIndex).toBe(1);
    expect(fixture.nativeElement.querySelectorAll('tr.mat-mdc-row').length).toBe(1);
    expect(tarefaService.obterHistorico).toHaveBeenCalledTimes(1);

    fixture.componentInstance.alternar();
    fixture.componentInstance.alternar();

    expect(tarefaService.obterHistorico).toHaveBeenCalledTimes(1);
  });
});

function criarMovimentacoes(quantidade: number): TarefaMovimentacao[] {
  return Array.from({ length: quantidade }, (_, indice) => ({
    id: indice + 1,
    movimento: 'Atualização',
    detalhes: `Movimentação ${indice + 1}`,
    responsavelCodigo: 'USR001',
    responsavelNome: 'Usuario Responsavel',
    dataOcorrencia: `2026-08-19T12:0${indice}:00`
  }));
}
