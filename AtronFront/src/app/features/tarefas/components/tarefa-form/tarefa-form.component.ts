import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { catchError, forkJoin, of } from 'rxjs';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { UsuarioInformacoesComponent } from '../../../../shared/components/usuario-informacoes/usuario-informacoes.component';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { UsuarioService } from '../../../usuarios/services/usuario.service';
import { EstadoTarefa } from '../../models/estadoTarefa.model';
import { TarefaService } from '../../services/tarefa.service';
import { UsuarioRequest } from '../../../usuarios/models/request/usuario-request';
import { UsuarioResponse } from '../../../usuarios/models/response/usuario-response';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { DateMaskDirective } from '../../../../shared/directives/date-mask.directive';
import { DestinoInicialTarefa } from '../../models/destino-inicial-tarefa.model';
import { DepartamentosService } from '../../../departamentos/services/departamentos.service';
import { CargoService } from '../../../cargos/services/cargo.service';
import { Departamento } from '../../../departamentos/models/departamento.model';
import { CargoModel } from '../../../cargos/models/cargo.model';
import { CargoResponse } from '../../../cargos/models/response/cargo-response.model';

@Component({
  selector: 'c-tarefa-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, UsuarioInformacoesComponent, DateMaskDirective],
  templateUrl: './tarefa-form.component.html',
  styleUrls: ['../../tarefa.component.css']
})

export class TarefaFormComponent implements OnInit {
  @Input() tarefaForm!: FormGroup;
  usuarioControl = new FormControl<string | UsuarioRequest | UsuarioResponse>('');
  estadoControl = new FormControl<string | EstadoTarefa>('');
  destinoControl = new FormControl<DestinoInicialTarefa>(DestinoInicialTarefa.getDestinos()[0]);
  departamentoControl = new FormControl<string | Departamento>('');
  cargoControl = new FormControl<string | CargoResponse | CargoModel>('');
  destinosIniciais = DestinoInicialTarefa.getDestinos();
  estadosDaTarefa: EstadoTarefa[] = EstadoTarefa.getEstados();
  usuarios: UsuarioResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];
  departamentosFiltrados: Departamento[] = [];
  todosDepartamentos: Departamento[] = [];
  cargosFiltrados: CargoResponse[] = [];
  todosCargos: CargoResponse[] = [];
  totalDeTarefas: number = 0;

  constructor(
    private service: TarefaService,
    private usuarioService: UsuarioService,
    private departamentoService: DepartamentosService,
    private cargoService: CargoService) { }

  ngOnInit(): void {
    forkJoin({
      usuarios: this.usuarioService.obterTodosUsuariosInformados(),
      departamentos: this.departamentoService.obterTodos(),
      cargos: this.cargoService.obterTodos(),
      estados: this.service.obterEstados().pipe(catchError(() => of(EstadoTarefa.getEstados())))
    }).subscribe(({ usuarios, departamentos, cargos, estados }) => {
      this.estadosDaTarefa = estados;
      this.todosUsuarios = usuarios;
      this.usuarios = this.todosUsuarios;
      this.todosDepartamentos = departamentos;
      this.departamentosFiltrados = this.todosDepartamentos;
      this.todosCargos = cargos;
      this.cargosFiltrados = this.todosCargos;

      const usuarioCodigo = this.tarefaForm.get('usuarioCodigo')?.value;
      const selecionado = this.todosUsuarios.find(u => u.codigo === usuarioCodigo);

      this.sincronizarUsuarioSelecionado(selecionado);
      this.sincronizarEstadoSelecionado();
      this.sincronizarDestinoSelecionado();
      this.sincronizarDepartamentoSelecionado();
      this.sincronizarCargoSelecionado();
    });

    this.destinoControl.valueChanges.subscribe(value => {
      this.tarefaForm.patchValue({ destinoInicial: value?.id ?? DestinoInicialTarefa.Usuario });
      this.limparEscopoIncompativel(value?.id ?? DestinoInicialTarefa.Usuario);
    });

    this.usuarioControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.usuarios = this.filtrarUsuarios(value);
        this.limparUsuarioSelecionado(false);
        return;
      }

      const usuario = value as UsuarioResponse | null;
      if (!usuario) {
        this.limparUsuarioSelecionado();
        return;
      }

      this.service.obterTodasTarefasRelacionadas().subscribe(tarefas => { this.totalDeTarefas = tarefas.filter(t => t.usuarioCodigo === usuario.codigo).length; });

      this.tarefaForm.patchValue({
        usuarioCodigo: usuario.codigo,
        usuarioNome: usuario.nome,
        cargoDescricao: formatLabel(usuario.cargo?.codigo, usuario.cargo?.descricao),
        departamentoDescricao: formatLabel(usuario.departamento?.codigo, usuario.departamento?.descricao)
      });
    });

    this.departamentoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.departamentosFiltrados = this.filtrarDepartamentos(value);
        this.tarefaForm.patchValue({ departamentoCodigo: null }, { emitEvent: false });
        return;
      }

      const departamento = value as Departamento | null;
      this.tarefaForm.patchValue({ departamentoCodigo: departamento?.codigo ?? null });
      this.cargosFiltrados = departamento
        ? this.todosCargos.filter(c => c.departamentoCodigo === departamento.codigo)
        : this.todosCargos;
    });

    this.cargoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.cargosFiltrados = this.filtrarCargos(value);
        this.tarefaForm.patchValue({ cargoCodigo: null }, { emitEvent: false });
        return;
      }

      const cargo = value as CargoResponse | CargoModel | null;
      this.tarefaForm.patchValue({
        cargoCodigo: cargo?.codigo ?? null,
        departamentoCodigo: cargo?.departamentoCodigo ?? this.tarefaForm.get('departamentoCodigo')?.value
      });
    });

    this.tarefaForm.get('usuarioCodigo')?.valueChanges.subscribe(codigo => {
      const selecionado = this.todosUsuarios.find(u => u.codigo === codigo);
      this.sincronizarUsuarioSelecionado(selecionado);
    });

    this.tarefaForm.get('destinoInicial')?.valueChanges.subscribe(() => {
      this.sincronizarDestinoSelecionado();
    });

    this.tarefaForm.get('departamentoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarDepartamentoSelecionado();
    });

    this.tarefaForm.get('cargoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarCargoSelecionado();
    });

    this.estadoControl.valueChanges.subscribe(value => {
      const estado = value as EstadoTarefa;
      this.tarefaForm.patchValue({
        estadoId: estado?.id,
        estadoDescricao: estado?.descricao
      });
    });
  }

  exibirUsuario(usuario: UsuarioResponse | string): string {
    if (typeof usuario === 'string') return usuario;
    return usuario ? usuario.nome + ' ' + usuario.sobrenome : '';
  }

  exibirEstado(estado: EstadoTarefa): string {
    return estado ? estado.descricao : '';
  }

  exibirDestino(destino: DestinoInicialTarefa): string {
    return destino ? destino.descricao : '';
  }

  exibirDepartamento(departamento: Departamento | string): string {
    return typeof departamento === 'string' ? departamento : departamento?.descricao ?? '';
  }

  exibirCargo(cargo: CargoResponse | CargoModel | string): string {
    return typeof cargo === 'string' ? cargo : cargo?.descricao ?? '';
  }

  private sincronizarEstadoSelecionado(): void {
    const estadoId = this.tarefaForm.get('estadoId')?.value;
    const estadoSelecionado = this.estadosDaTarefa.find(e => e.id === estadoId);
    if (estadoSelecionado) {
      this.estadoControl.setValue(estadoSelecionado);
    }
  }

  private sincronizarDestinoSelecionado(): void {
    const destinoInicial = this.tarefaForm.get('destinoInicial')?.value || DestinoInicialTarefa.Usuario;
    const selecionado = this.destinosIniciais.find(destino => destino.id === destinoInicial);
    if (selecionado) {
      this.destinoControl.setValue(selecionado, { emitEvent: false });
    }
  }

  private filtrarUsuarios(valor: string): UsuarioResponse[] {
    const filtro = valor.toLowerCase();

    return this.todosUsuarios.filter(usuario =>
      usuario.codigo?.toLowerCase().includes(filtro) ||
      usuario.nome?.toLowerCase().includes(filtro) ||
      usuario.sobrenome?.toLowerCase().includes(filtro) ||
      `${usuario.nome} ${usuario.sobrenome}`.toLowerCase().includes(filtro)
    );
  }

  private filtrarDepartamentos(valor: string): Departamento[] {
    const filtro = valor.toLowerCase();

    return this.todosDepartamentos.filter(departamento =>
      departamento.codigo?.toLowerCase().includes(filtro) ||
      departamento.descricao?.toLowerCase().includes(filtro)
    );
  }

  private filtrarCargos(valor: string): CargoResponse[] {
    const filtro = valor.toLowerCase();
    const departamentoCodigo = this.tarefaForm.get('departamentoCodigo')?.value;
    const cargos = departamentoCodigo
      ? this.todosCargos.filter(cargo => cargo.departamentoCodigo === departamentoCodigo)
      : this.todosCargos;

    return cargos.filter(cargo =>
      cargo.codigo?.toLowerCase().includes(filtro) ||
      cargo.descricao?.toLowerCase().includes(filtro)
    );
  }

  private sincronizarUsuarioSelecionado(usuario?: UsuarioResponse): void {
    this.usuarioControl.setValue(usuario || '', { emitEvent: false });
    this.usuarios = usuario ? [usuario] : this.todosUsuarios;
  }

  private limparUsuarioSelecionado(restaurarLista: boolean = true): void {
    this.totalDeTarefas = 0;
    if (restaurarLista) {
      this.usuarios = this.todosUsuarios;
    }
    this.tarefaForm.patchValue({
      usuarioCodigo: null,
      usuarioNome: '',
      cargoDescricao: '',
      departamentoDescricao: ''
    }, { emitEvent: false });
  }

  private sincronizarDepartamentoSelecionado(): void {
    const departamentoCodigo = this.tarefaForm.get('departamentoCodigo')?.value;
    const selecionado = this.todosDepartamentos.find(d => d.codigo === departamentoCodigo);

    this.departamentoControl.setValue(selecionado || '', { emitEvent: false });
    this.departamentosFiltrados = selecionado ? [selecionado] : this.todosDepartamentos;
  }

  private sincronizarCargoSelecionado(): void {
    const cargoCodigo = this.tarefaForm.get('cargoCodigo')?.value;
    const selecionado = this.todosCargos.find(c => c.codigo === cargoCodigo);

    this.cargoControl.setValue(selecionado || '', { emitEvent: false });
    this.cargosFiltrados = selecionado ? [selecionado] : this.todosCargos;
  }

  private limparEscopoIncompativel(destinoInicial: number): void {
    if (destinoInicial !== DestinoInicialTarefa.Usuario) {
      this.limparUsuarioSelecionado();
    }

    if (destinoInicial !== DestinoInicialTarefa.DepartamentoCargo) {
      this.tarefaForm.patchValue({
        departamentoCodigo: null,
        cargoCodigo: null
      }, { emitEvent: false });
      this.departamentoControl.setValue('', { emitEvent: false });
      this.cargoControl.setValue('', { emitEvent: false });
    }
  }
}
