import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { AcessoService } from '../../../../core/services/acesso.service';
import { NotificacaoService } from '../../../../core/services/notification.service';
import { SessaoInfoService } from '../../../../core/services/sessaoInfo.service';
import { VisualizacaoService } from '../../../../core/services/visualizacao-service';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  it('deve enviar somente uma requisição enquanto o login estiver em andamento', () => {
    const requisicao = new Subject<void>();
    const acessoService = jasmine.createSpyObj<AcessoService>('AcessoService', ['autenticar']);
    const sessaoService = jasmine.createSpyObj<SessaoInfoService>('SessaoInfoService', [
      'obterUsuarioCodigo',
      'clearSessionInfo'
    ]);
    const notificacaoService = jasmine.createSpyObj<NotificacaoService>('NotificacaoService', [
      'normalizarMensagens',
      'exibirMensagens',
      'exibirMensagem'
    ]);
    const router = jasmine.createSpyObj<Router>('Router', ['navigate']);
    const visualizacaoService = jasmine.createSpyObj<VisualizacaoService>('VisualizacaoService', [
      'getViewMode',
      'setViewMode'
    ]);

    acessoService.autenticar.and.returnValue(requisicao);
    sessaoService.obterUsuarioCodigo.and.returnValue(null);

    const component = new LoginComponent(
      new FormBuilder(),
      acessoService,
      sessaoService,
      notificacaoService,
      router,
      visualizacaoService
    );
    component.ngOnInit();
    component.form.setValue({ codigo: 'USR001', senha: 'senha' });

    component.autenticar();
    component.autenticar();

    expect(acessoService.autenticar).toHaveBeenCalledTimes(1);
    expect(component.autenticando).toBeTrue();

    requisicao.complete();

    expect(component.autenticando).toBeFalse();
  });
});
