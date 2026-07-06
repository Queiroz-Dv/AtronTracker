import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { CargoService } from '../../../cargos/services/cargo.service';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { CargoModel } from '../../../cargos/models/cargo.model';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';
import { DateMaskDirective } from '../../../../shared/directives/date-mask.directive';
import { UsuarioService } from '../../services/usuario.service';
import { UsuarioResponse, exibirDescricaoDoUsuario } from '../../models/response/usuario-response';

@Component({
  selector: 'c-usuario-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, DateMaskDirective],
  templateUrl: './usuario-form.component.html',
})

export class UsuarioFormComponent implements OnInit {
  @Input() usuarioForm!: FormGroup;

  // Controles individuais para autocomplete
  departamentoControl = new FormControl<string | Departamento>('');
  cargoControl = new FormControl<string | CargoResponse | CargoModel>('');
  gestorImediatoControl = new FormControl<string | UsuarioResponse>('');

  // Dados filtrados para os autocompletes
  departamentosFiltrados: Departamento[] = [];
  cargosFiltrados: CargoResponse[] = [];

  // Backup dos dados originais
  todosDepartamentos: Departamento[] = [];
  todosCargos: CargoResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];
  gestoresFiltrados: UsuarioResponse[] = [];

  constructor(
    private cargoService: CargoService,
    private departamentoService: DepartamentosService,
    private usuarioService: UsuarioService
  ) { }

  ngOnInit(): void {
    this.departamentoService.obterTodos().subscribe(deps => {
      this.todosDepartamentos = deps;
      this.departamentosFiltrados = this.todosDepartamentos;
      this.sincronizarDepartamentoSelecionado();
    });

    this.cargoService.obterTodos().subscribe(crgs => {
      this.todosCargos = crgs;
      this.cargosFiltrados = this.todosCargos;
      this.sincronizarCargoSelecionado();
    });

    this.usuarioService.obterTodosUsuariosInformados().subscribe(usuarios => {
      this.todosUsuarios = usuarios;
      this.gestoresFiltrados = this.filtrarGestoresDisponiveis('');
      this.sincronizarGestorSelecionado();
    });

    this.departamentoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        // Usuário está digitando - aplica filtro de busca
        this.departamentosFiltrados = this._filtrarDepartamentos(value);
      } else if (value) {
        // Usuário selecionou um item
        const departamento = value as Departamento;
        this.cargosFiltrados = this.todosCargos.filter(c => c.departamentoCodigo === departamento.codigo);
        this.usuarioForm.patchValue({ departamentoCodigo: departamento.codigo });
      } else {
        this.usuarioForm.patchValue({ departamentoCodigo: null });
        this.cargosFiltrados = this.todosCargos;
      }
    });

    this.cargoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        // Usuário está digitando - aplica filtro de busca
        this.cargosFiltrados = this._filtrarCargos(value);
      } else if (value) {
        // Usuário selecionou um item
        const cargo = value as CargoModel;
        this.departamentosFiltrados = this.todosDepartamentos.filter(d => d.codigo === cargo.departamentoCodigo);
        this.usuarioForm.patchValue({
          cargoCodigo: cargo.codigo,
          departamentoCodigo: cargo.departamentoCodigo
        });
      } else {
        this.usuarioForm.patchValue({ cargoCodigo: null });
        this.departamentosFiltrados = this.todosDepartamentos;
      }
    });

    this.gestorImediatoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.gestoresFiltrados = this.filtrarGestoresDisponiveis(value);
        this.usuarioForm.patchValue({ gestorImediatoCodigo: null }, { emitEvent: false });
        return;
      }

      const gestor = value as UsuarioResponse | null;
      this.usuarioForm.patchValue({ gestorImediatoCodigo: gestor?.codigo ?? null });
    });

    this.usuarioForm.get('departamentoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarDepartamentoSelecionado();
    });

    this.usuarioForm.get('cargoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarCargoSelecionado();
    });

    this.usuarioForm.get('gestorImediatoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarGestorSelecionado();
    });
  }

  private sincronizarDepartamentoSelecionado(): void {
    const departamentoCodigo = this.usuarioForm.get('departamentoCodigo')?.value;
    const selecionado = this.todosDepartamentos.find(d => d.codigo === departamentoCodigo);

    this.departamentoControl.setValue(selecionado || '', { emitEvent: false });
    this.departamentosFiltrados = selecionado ? [selecionado] : this.todosDepartamentos;

    if (selecionado) {
      this.cargosFiltrados = this.todosCargos.filter(c => c.departamentoCodigo === selecionado.codigo);
    }
  }

  private sincronizarCargoSelecionado(): void {
    const cargoCodigo = this.usuarioForm.get('cargoCodigo')?.value;
    const selecionado = this.todosCargos.find(c => c.codigo === cargoCodigo);

    this.cargoControl.setValue(selecionado || '', { emitEvent: false });

    if (!selecionado) {
      this.cargosFiltrados = this.todosCargos;
      return;
    }

    this.cargosFiltrados = [selecionado];

    const departamentoSelecionado = this.todosDepartamentos.find(d => d.codigo === selecionado.departamentoCodigo);
    if (departamentoSelecionado) {
      this.departamentoControl.setValue(departamentoSelecionado, { emitEvent: false });
      this.departamentosFiltrados = [departamentoSelecionado];
    }
  }

  private _filtrarDepartamentos(valor: string): Departamento[] {
    const filtro = valor.toLowerCase();
    return this.todosDepartamentos.filter(dep => 
      dep.descricao.toLowerCase().includes(filtro) || 
      dep.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  private _filtrarCargos(valor: string): CargoResponse[] {
    const filtro = valor.toLowerCase();
    return this.todosCargos.filter(crg => 
      crg.descricao.toLowerCase().includes(filtro) || 
      crg.codigo.toString().toLowerCase().includes(filtro)
    );
  }

  private sincronizarGestorSelecionado(): void {
    const gestorCodigo = this.usuarioForm.get('gestorImediatoCodigo')?.value;
    const selecionado = this.todosUsuarios.find(u => u.codigo === gestorCodigo);

    this.gestorImediatoControl.setValue(selecionado || '', { emitEvent: false });
    this.gestoresFiltrados = selecionado ? [selecionado] : this.filtrarGestoresDisponiveis('');
  }

  private filtrarGestoresDisponiveis(valor: string): UsuarioResponse[] {
    const filtro = valor.toLowerCase();
    const codigoUsuarioAtual = this.usuarioForm.get('codigo')?.value;

    return this.todosUsuarios.filter(usuario =>
      usuario.codigo !== codigoUsuarioAtual &&
      (
        usuario.codigo?.toLowerCase().includes(filtro) ||
        usuario.nome?.toLowerCase().includes(filtro) ||
        usuario.sobrenome?.toLowerCase().includes(filtro) ||
        `${usuario.nome} ${usuario.sobrenome}`.toLowerCase().includes(filtro)
      )
    );
  }

  exibirDepartamentoDescricao = (dpt: Departamento | string) => typeof dpt === 'string' ? dpt : dpt?.descricao ?? '';
  exibirCargoDescricao = (crg: CargoModel | CargoResponse | string) => typeof crg === 'string' ? crg : crg?.descricao ?? '';
  exibirGestorDescricao = (usuario: UsuarioResponse | string) => typeof usuario === 'string' ? usuario : exibirDescricaoDoUsuario(usuario);
}
