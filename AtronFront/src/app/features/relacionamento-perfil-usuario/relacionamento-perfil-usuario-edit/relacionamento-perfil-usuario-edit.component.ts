import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SharedModule } from '../../../shared/modules/shared.module';
import { UsuarioPerfilDTO } from '../models/perfil-usuario.model';
import { ActivatedRoute, Router } from '@angular/router';
import { PerfilDeAcessoService } from '../../perfil-de-acesso/services/perfil-de-acesso.service';
import { PerfilDeAcessoModel } from '../../perfil-de-acesso/interfaces/perfil-de-acesso.interface';
import { UsuarioService } from '../../usuarios/services/usuario.service';
import { MatCheckbox } from '@angular/material/checkbox';
import { BotaoVoltarComponent } from "../../../core/layout/botao-voltar/botao-voltar.component";
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'c-relacionamento-perfil-usuario-edit',
  imports: [SharedModule, ReactiveFormsModule, MatCheckbox, BotaoVoltarComponent],
  templateUrl: './relacionamento-perfil-usuario-edit.component.html',
  styleUrl: '../relacionamento-perfil-usuario.component.css'
})
export class RelacionamentoPerfilUsuarioEditComponent implements OnInit, AfterViewInit {
  form!: FormGroup
  perfis: PerfilDeAcessoModel[] = [];
  usuarios: UsuarioPerfilDTO[] = [];
  datasource = new MatTableDataSource<UsuarioPerfilDTO>([]);
  displayedColumns = ['codigo', 'nome', 'sobrenome'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private fb: FormBuilder,
    private perfilService: PerfilDeAcessoService,
    private usuarioService: UsuarioService,
    private route: ActivatedRoute,
    public router: Router) { }


  ngOnInit(): void {
    this.form = this.fb.group({
      codigoPerfil: [null, Validators.required],
      usuarios: [[]]    // string[] de códigos
    });

    // carrega dropdown de perfis
    this.perfilService.obterTodos().subscribe(list => {
      this.perfis = list;
    });

    // carrega lista de usuários e popula tabela
    this.usuarioService.obterTodos().subscribe(list => {
      this.usuarios = list.map(u => ({
        codigo: u.codigo,
        nome: u.nome,
        sobrenome: u.sobrenome
      }));

      this.datasource.data = list;
    });

    // se for edição, pega :codigoPerfil
    const codigo = this.route.snapshot.paramMap.get('codigoPerfil');
    if (codigo) {
      this.form.get('codigoPerfil')!.setValue(codigo);
      this.loadRelacionamentos(codigo);
    }

    // ao trocar de perfil no select, recarrega marcações
    this.form.get('codigoPerfil')!.valueChanges.subscribe(sel => {
      if (sel) this.loadRelacionamentos(sel.codigo);
      else this.form.get('usuarios')!.setValue([]);
    });
  }

  ngAfterViewInit(): void {
    // só aqui o paginator e o sort já existem
    this.datasource.paginator = this.paginator;
    this.datasource.sort = this.sort;
  }

  private loadRelacionamentos(codigoPerfil: string) {
    this.perfilService
      .obterRelacionamentoPorCodigoDoPerfil(codigoPerfil)
      .subscribe({
        next: rel => {
          console.log(rel);
          const codigos = rel.usuarios.map(u => u.codigo);
          this.form.get('usuarios')!.setValue(codigos);
        },
        error: () => {
          this.form.get('usuarios')!.setValue([]);
        }
      });
  }

  // helpers para o checkbox
  estaSelecionado(codigoUsuario: string): boolean {
    return (this.form.value.usuarios as string[]).includes(codigoUsuario);
  }

  onToggleUsuario(codigoUsuario: string, checked: boolean) {
    const ctrl = this.form.get('usuarios')!;
    const list = [...ctrl.value as string[]];
    if (checked) {
      if (!list.includes(codigoUsuario)) list.push(codigoUsuario);
    } else {
      ctrl.setValue(list.filter(c => c !== codigoUsuario));
      return;
    }
    ctrl.setValue(list);
  }

  exibirPerfil(p?: PerfilDeAcessoModel): string {
    return p ? `${p.codigo} – ${p.descricao}` : '';
  }

  salvar() {
    const perfil = this.form.get('codigoPerfil').value;
    const usuariosForm = this.form.get('usuarios').value;

    const payload: PerfilUsuarioRequest = {
      codigoPerfil: perfil.codigo,
      usuarios: (usuariosForm as string[]).map(codigoUsuario => ({ codigo: codigoUsuario }))
    };

    const operacao = this.perfilService.gravarRelacionamento(payload);
    this.form.reset({
      codigoPerfil: null,
      usuarios: []
    });

    operacao.subscribe(() => this.router.navigate(['atron/perfil-de-acesso/relacionamento-perfil-usuario/novo']));
  }
}

export interface PerfilUsuarioRequest {
  codigoPerfil: string;
  usuarios: { codigo: string }[];
}
