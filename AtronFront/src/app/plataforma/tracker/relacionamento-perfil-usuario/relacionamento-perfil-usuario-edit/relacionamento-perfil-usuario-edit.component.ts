import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { BotaoVoltarComponent } from '../../../../core/layout/botao-voltar/botao-voltar.component';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { PerfilDeAcessoModel } from '../../perfil-de-acesso/interfaces/perfil-de-acesso.interface';
import { PerfilDeAcessoService } from '../../perfil-de-acesso/services/perfil-de-acesso.service';
import { UsuarioService } from '../../usuarios/services/usuario.service';
import { UsuarioPerfilDTO } from '../models/perfil-usuario.model';

@Component({
  selector: 'c-relacionamento-perfil-usuario-edit',
  imports: [SharedModule, ReactiveFormsModule, MatCheckbox, BotaoVoltarComponent],
  templateUrl: './relacionamento-perfil-usuario-edit.component.html',
  styleUrl: '../relacionamento-perfil-usuario.component.css'
})
export class RelacionamentoPerfilUsuarioEditComponent implements OnInit, AfterViewInit {
  form!: FormGroup;
  perfis: PerfilDeAcessoModel[] = [];
  perfisFiltrados: PerfilDeAcessoModel[] = [];
  usuarios: UsuarioPerfilDTO[] = [];
  buscaUsuarioControl = new FormControl('');
  datasource = new MatTableDataSource<UsuarioPerfilDTO>([]);
  displayedColumns = ['codigo', 'nome', 'sobrenome'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private fb: FormBuilder,
    private perfilService: PerfilDeAcessoService,
    private usuarioService: UsuarioService,
    private route: ActivatedRoute,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigoPerfil: [null, Validators.required],
      usuarios: [[]]
    });

    this.datasource.filterPredicate = (usuario, filtro) => {
      const termo = filtro.trim().toLowerCase();
      return usuario.codigo?.toLowerCase().includes(termo)
        || usuario.nome?.toLowerCase().includes(termo)
        || usuario.sobrenome?.toLowerCase().includes(termo)
        || `${usuario.nome} ${usuario.sobrenome}`.toLowerCase().includes(termo);
    };

    this.perfilService.obterTodos().subscribe(perfis => {
      this.perfis = perfis;
      this.perfisFiltrados = perfis;
      this.sincronizarPerfilDaRota();
    });

    this.usuarioService.obterTodosUsuariosInformados().subscribe(usuarios => {
      this.usuarios = usuarios.map(usuario => ({
        codigo: usuario.codigo,
        nome: usuario.nome,
        sobrenome: usuario.sobrenome
      }));

      this.datasource.data = this.usuarios;
    });

    this.buscaUsuarioControl.valueChanges.subscribe(valor => this.filtrarUsuarios(valor ?? ''));

    this.form.get('codigoPerfil')!.valueChanges.subscribe(valor => {
      this.perfisFiltrados = this.filtrarPerfis(valor);

      const codigoPerfil = this.obterCodigoPerfilValido(valor);
      if (codigoPerfil) {
        this.loadRelacionamentos(codigoPerfil);
      } else {
        this.form.get('usuarios')!.setValue([]);
      }
    });
  }

  ngAfterViewInit(): void {
    this.datasource.paginator = this.paginator;
    this.datasource.sort = this.sort;
  }

  estaSelecionado(codigoUsuario: string): boolean {
    return (this.form.value.usuarios as string[]).includes(codigoUsuario);
  }

  onToggleUsuario(codigoUsuario: string, checked: boolean): void {
    const ctrl = this.form.get('usuarios')!;
    const codigosSelecionados = [...ctrl.value as string[]];

    if (checked) {
      if (!codigosSelecionados.includes(codigoUsuario)) {
        codigosSelecionados.push(codigoUsuario);
      }

      ctrl.setValue(codigosSelecionados);
      return;
    }

    ctrl.setValue(codigosSelecionados.filter(codigo => codigo !== codigoUsuario));
  }

  exibirPerfil(perfil?: PerfilDeAcessoModel | string): string {
    if (!perfil) return '';
    if (typeof perfil === 'string') return perfil;
    return `${perfil.codigo} - ${perfil.descricao}`;
  }

  salvar(): void {
    const codigoPerfil = this.obterCodigoPerfilValido(this.form.get('codigoPerfil')!.value);
    const usuariosForm = this.form.get('usuarios')!.value as string[];

    if (!codigoPerfil) {
      this.form.get('codigoPerfil')?.setErrors({ required: true });
      return;
    }

    const payload: PerfilUsuarioRequest = {
      codigoPerfil,
      usuarios: usuariosForm.map(codigoUsuario => ({ codigo: codigoUsuario }))
    };

    this.perfilService
      .gravarRelacionamento(payload)
      .subscribe(() => {
        this.form.reset({ codigoPerfil: null, usuarios: [] });
        this.buscaUsuarioControl.setValue('');
        this.router.navigate(['atron/perfil-de-acesso/relacionamento-perfil-usuario/novo']);
      });
  }

  private loadRelacionamentos(codigoPerfil: string): void {
    this.perfilService
      .obterRelacionamentoPorCodigoDoPerfil(codigoPerfil)
      .subscribe({
        next: relacionamento => {
          const codigos = relacionamento.usuarios?.map(usuario => usuario.codigo) ?? [];
          this.form.get('usuarios')!.setValue(codigos);
        },
        error: () => {
          this.form.get('usuarios')!.setValue([]);
        }
      });
  }

  private sincronizarPerfilDaRota(): void {
    const codigo = this.route.snapshot.paramMap.get('codigoPerfil');
    if (!codigo) return;

    const perfil = this.perfis.find(item => item.codigo === codigo);
    this.form.get('codigoPerfil')!.setValue(perfil ?? codigo);
    this.loadRelacionamentos(codigo);
  }

  private filtrarPerfis(valor: PerfilDeAcessoModel | string | null): PerfilDeAcessoModel[] {
    const termo = typeof valor === 'string'
      ? valor.trim().toLowerCase()
      : valor?.codigo?.toLowerCase() ?? '';

    if (!termo) return this.perfis;

    return this.perfis.filter(perfil =>
      perfil.codigo?.toLowerCase().includes(termo)
      || perfil.descricao?.toLowerCase().includes(termo)
    );
  }

  private filtrarUsuarios(valor: string): void {
    this.datasource.filter = valor.trim().toLowerCase();
    this.datasource.paginator?.firstPage();
  }

  private obterCodigoPerfilValido(valor: PerfilDeAcessoModel | string | null): string | null {
    if (!valor) return null;
    if (typeof valor === 'string') {
      const perfil = this.perfis.find(item => item.codigo === valor);
      return perfil?.codigo ?? null;
    }

    return valor.codigo;
  }
}

export interface PerfilUsuarioRequest {
  codigoPerfil: string;
  usuarios: { codigo: string }[];
}
