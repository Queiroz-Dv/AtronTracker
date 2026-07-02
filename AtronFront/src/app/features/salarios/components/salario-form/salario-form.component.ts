import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MesDto } from '../../../models/mes.dto';
import { UsuarioService } from '../../../usuarios/services/usuario.service';
import { ControlErrorComponent } from '../../../../shared/components/control-error/control-error.component';
import { UsuarioInformacoesComponent } from '../../../../shared/components/usuario-informacoes/usuario-informacoes.component';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { exibirDescricaoDoUsuario, UsuarioResponse } from '../../../usuarios/models/response/usuario-response';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { UsuarioRequest } from '../../../usuarios/models/request/usuario-request';

@Component({
  selector: 'c-salario-form',
  imports: [ReactiveFormsModule, UsuarioInformacoesComponent, SharedModule, ControlErrorComponent],
  templateUrl: './salario-form.component.html',
  styleUrls: ['../../salario.component.css']
})

export class SalarioFormComponent implements OnInit {
  @Input() salarioForm!: FormGroup;
  usuarioControl = new FormControl<string | UsuarioRequest | UsuarioResponse>('');
  meses: MesDto[] = MesDto.getMeses();
  mesControl = new FormControl<string | MesDto>('');
  usuarios: UsuarioResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];

  constructor(private usuarioService: UsuarioService) { }

  ngOnInit(): void {
    this.usuarioService.obterTodosUsuariosInformados().subscribe(usuarios => {
      this.todosUsuarios = usuarios;
      this.usuarios = this.todosUsuarios;

      const usuarioCodigo = this.salarioForm.get('usuarioCodigo')?.value;
      const selecionado = this.todosUsuarios.find(u => u.codigo === usuarioCodigo);
      this.sincronizarUsuarioSelecionado(selecionado);

      const mesId = this.salarioForm.get('mesId')?.value;
      const mesSelecionado = this.meses.find(ms => ms.id === mesId);
      if (mesSelecionado) {
        this.mesControl.setValue(mesSelecionado);
      }
    });

    this.mesControl.valueChanges.subscribe(value => {
      const mes = value as MesDto;
      this.salarioForm.patchValue({
        mesId: mes.id,
        mesDescricao: mes.descricao
      });
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

      this.salarioForm.patchValue({
        usuarioCodigo: usuario.codigo,
        usuarioNome: usuario.nome,
        cargoDescricao: formatLabel(usuario.cargo?.codigo, usuario.cargo?.descricao),
        departamentoDescricao: formatLabel(usuario.departamento?.codigo, usuario.departamento?.descricao)
      });

    });

    this.salarioForm.get('usuarioCodigo')?.valueChanges.subscribe(codigo => {
      const selecionado = this.todosUsuarios.find(u => u.codigo === codigo);
      this.sincronizarUsuarioSelecionado(selecionado);
    });
  }

  get isUsuarioDesabilitado(): boolean {
    return this.usuarioControl.disabled;
  }

  desabilitarUsuarioSelect() {
    this.usuarioControl.disable();
  }

  exibirUsuario(usuario: UsuarioResponse | string): string {
    if (typeof usuario === 'string') return usuario;
    return exibirDescricaoDoUsuario(usuario);
  }

  exibirMes(mes: MesDto): string {
    return mes ? mes.descricao : '';
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
    if (restaurarLista) {
      this.usuarios = this.todosUsuarios;
    }

    this.salarioForm.patchValue({
      usuarioCodigo: null,
      usuarioNome: '',
      cargoDescricao: '',
      departamentoDescricao: ''
    }, { emitEvent: false });
  }
}
