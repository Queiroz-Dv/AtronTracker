import { FormControl, FormGroup } from '@angular/forms';
import { ModuloModel } from '../../../../../features/navegacao/modulos/interfaces/modulo.interface';
import { PerfilDeAcessoFormComponent } from './perfil-de-acesso-form.component';

describe('PerfilDeAcessoFormComponent', () => {
  it('deve exibir todo o catálogo e marcar somente os módulos vinculados ao perfil', () => {
    const todosModulos: ModuloModel[] = [
      { codigo: 'DPT', descricao: 'Departamentos' },
      { codigo: 'USR', descricao: 'Usuários' },
      { codigo: 'PRD', descricao: 'Produtos' }
    ];
    const component = new PerfilDeAcessoFormComponent();
    component.form = new FormGroup({
      modulos: new FormControl(['DPT', 'USR'])
    });
    component.todosModulos = todosModulos;
    component.modulosDoPerfil = todosModulos.slice(0, 2);
    component.perfilEncontrado = true;

    component.ngOnInit();
    component.ngOnChanges({});

    expect(component.dataSource.data).toEqual(todosModulos);
    expect(component.estaSelecionado('DPT')).toBeTrue();
    expect(component.estaSelecionado('USR')).toBeTrue();
    expect(component.estaSelecionado('PRD')).toBeFalse();
  });
});
