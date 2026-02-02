import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { UsuarioInformacoesComponent } from '../../../../shared/components/usuario-informacoes/usuario-informacoes.component';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { UsuarioService } from '../../../usuarios/services/usuario.service';
import { EstadoTarefa } from '../../models/estadoTarefa.model';
import { TarefaService } from '../../services/tarefa.service';
import { UsuarioRequest } from '../../../usuarios/models/request/usuario-request';
import { UsuarioResponse } from '../../../usuarios/models/response/usuario-response';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';

@Component({
  selector: 'c-tarefa-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent, UsuarioInformacoesComponent],
  templateUrl: './tarefa-form.component.html',
  styleUrls: ['../../tarefa.component.css']
})

export class TarefaFormComponent implements OnInit {
  @Input() tarefaForm!: FormGroup;
  usuarioControl = new FormControl<string | UsuarioRequest>('');
  estadoControl = new FormControl<string | EstadoTarefa>('');
  estadosDaTarefa: EstadoTarefa[] = EstadoTarefa.getEstados();
  usuarios: UsuarioResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];
  totalDeTarefas: number = 0;

  constructor(private service: TarefaService, private usuarioService: UsuarioService,) { }

  ngOnInit(): void {
    this.usuarioService.obterTodosUsuariosInformados().subscribe(usuarios => {
      this.todosUsuarios = usuarios;
      this.usuarios = this.todosUsuarios;

      const usuarioCodigo = this.tarefaForm.get('usuarioCodigo')?.value;
      const selecionado = this.todosUsuarios.find(u => u.codigo === usuarioCodigo);

      this.usuarioControl.setValue(selecionado || '');

      const estadoId = this.tarefaForm.get('estadoId')?.value;
      const estadoSelecionado = this.estadosDaTarefa.find(e => e.id === estadoId);
      if (estadoSelecionado) {
        this.estadoControl.setValue(estadoSelecionado);
      }
    });

    this.usuarioControl.valueChanges.subscribe(value => {
      const usuario = value as UsuarioResponse;
      if (!usuario) { return; }

      this.service.obterTodasTarefasRelacionadas().subscribe(tarefas => { this.totalDeTarefas = tarefas.filter(t => t.usuarioCodigo === usuario.codigo).length; });

      this.tarefaForm.patchValue({
        usuarioCodigo: usuario.codigo,
        usuarioNome: usuario.nome,
        cargoDescricao: formatLabel(usuario.cargo?.codigo, usuario.cargo?.descricao),
        departamentoDescricao: formatLabel(usuario.departamento?.codigo, usuario.departamento?.descricao)
      });
    });

    this.estadoControl.valueChanges.subscribe(value => {
      const estado = value as EstadoTarefa;
      this.tarefaForm.patchValue({
        estadoId: estado?.id,
        estadoDescricao: estado?.descricao
      });
    });
  }

  exibirUsuario(usuario: UsuarioResponse): string {
    return usuario ? usuario.nome + ' ' + usuario.sobrenome : '';
  }

  exibirEstado(estado: EstadoTarefa): string {
    return estado ? estado.descricao : '';
  }
}