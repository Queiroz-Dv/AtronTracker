import { Component, Input, OnInit, } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, } from '@angular/forms';
import { ControlErrorComponent } from "../../../../shared/components/control-error/control-error.component";
import { SharedModule } from '../../../../shared/modules/shared.module';
import { UsuarioService } from '../../../usuarios/services/usuario.service';
import { UsuarioResponse, exibirDescricaoDoUsuario } from '../../../usuarios/models/response/usuario-response';

@Component({
  standalone: true,
  selector: 'c-departamento-form',
  imports: [SharedModule, ReactiveFormsModule, ControlErrorComponent],
  templateUrl: './departamento-form.component.html',
})

export class DepartamentoFormComponent implements OnInit {
  @Input() form!: FormGroup;

  gestorDepartamentoControl = new FormControl<string | UsuarioResponse>('');
  gestoresFiltrados: UsuarioResponse[] = [];
  todosUsuarios: UsuarioResponse[] = [];

  constructor(private usuarioService: UsuarioService) { }

  ngOnInit(): void {
    this.usuarioService.obterTodosUsuariosInformados().subscribe(usuarios => {
      this.todosUsuarios = usuarios;
      this.gestoresFiltrados = this.filtrarGestores('');
      this.sincronizarGestorSelecionado();
    });

    this.gestorDepartamentoControl.valueChanges.subscribe(value => {
      if (typeof value === 'string') {
        this.gestoresFiltrados = this.filtrarGestores(value);
        this.form.patchValue({ gestorDepartamentoCodigo: null }, { emitEvent: false });
        return;
      }

      const gestor = value as UsuarioResponse | null;
      this.form.patchValue({ gestorDepartamentoCodigo: gestor?.codigo ?? null });
    });

    this.form.get('gestorDepartamentoCodigo')?.valueChanges.subscribe(() => {
      this.sincronizarGestorSelecionado();
    });
  }

  private sincronizarGestorSelecionado(): void {
    const gestorCodigo = this.form.get('gestorDepartamentoCodigo')?.value;
    const selecionado = this.todosUsuarios.find(u => u.codigo === gestorCodigo);

    this.gestorDepartamentoControl.setValue(selecionado || '', { emitEvent: false });
    this.gestoresFiltrados = selecionado ? [selecionado] : this.filtrarGestores('');
  }

  private filtrarGestores(valor: string): UsuarioResponse[] {
    const filtro = valor.toLowerCase();

    return this.todosUsuarios.filter(usuario =>
      usuario.codigo?.toLowerCase().includes(filtro) ||
      usuario.nome?.toLowerCase().includes(filtro) ||
      usuario.sobrenome?.toLowerCase().includes(filtro) ||
      `${usuario.nome} ${usuario.sobrenome}`.toLowerCase().includes(filtro)
    );
  }

  exibirGestorDescricao = (usuario: UsuarioResponse | string) => typeof usuario === 'string' ? usuario : exibirDescricaoDoUsuario(usuario);
}
