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
  estadosDaTarefa: EstadoTarefa[] = EstadoTarefa.getEstados();
  usuarios: UsuarioResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];
  totalDeTarefas: number = 0;

  constructor(private service: TarefaService, private usuarioService: UsuarioService,) { }

  ngOnInit(): void {
    forkJoin({
      usuarios: this.usuarioService.obterTodosUsuariosInformados(),
      estados: this.service.obterEstados().pipe(catchError(() => of(EstadoTarefa.getEstados())))
    }).subscribe(({ usuarios, estados }) => {
      this.estadosDaTarefa = estados;
      this.todosUsuarios = usuarios;
      this.usuarios = this.todosUsuarios;

      const usuarioCodigo = this.tarefaForm.get('usuarioCodigo')?.value;
      const selecionado = this.todosUsuarios.find(u => u.codigo === usuarioCodigo);

      this.sincronizarUsuarioSelecionado(selecionado);
      this.sincronizarEstadoSelecionado();
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

    this.tarefaForm.get('usuarioCodigo')?.valueChanges.subscribe(codigo => {
      const selecionado = this.todosUsuarios.find(u => u.codigo === codigo);
      this.sincronizarUsuarioSelecionado(selecionado);
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

  private sincronizarEstadoSelecionado(): void {
    const estadoId = this.tarefaForm.get('estadoId')?.value;
    const estadoSelecionado = this.estadosDaTarefa.find(e => e.id === estadoId);
    if (estadoSelecionado) {
      this.estadoControl.setValue(estadoSelecionado);
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
}
