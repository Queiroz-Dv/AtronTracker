import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { finalize } from 'rxjs';
import { AcessoService } from '../../../../core/services/acesso.service';
import { SessaoInfoService } from '../../../../core/services/sessaoInfo.service';
import { NotificacaoService } from '../../../../core/services/notification.service';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { ModuloModel } from '../../../../features/navegacao/modulos/interfaces/modulo.interface';
import { ModuloService } from '../../../../features/navegacao/modulos/services/modulo.service';

@Component({
  standalone: true,
  selector: 'c-configurar-perfil-inicial',
  imports: [SharedModule, ReactiveFormsModule],
  templateUrl: './configurar-perfil-inicial.component.html',
  styleUrl: './configurar-perfil-inicial.component.css'
})
export class ConfigurarPerfilInicialComponent implements OnInit {
  form!: FormGroup;
  salvando = false;
  carregandoModulos = false;

  usuarioCodigo = '';
  workspaceCodigo = '';
  usuarioNome = '';

  todosModulos: ModuloModel[] = [];
  dataSource = new MatTableDataSource<ModuloModel>([]);
  buscaModuloControl = new FormControl('');
  columnsToDisplay = ['selecionar', 'moduloCodigo', 'moduloDescricao'];

  @ViewChild(MatPaginator) set paginator(value: MatPaginator) {
    this.dataSource.paginator = value;
  }

  @ViewChild(MatSort) set sort(value: MatSort) {
    this.dataSource.sort = value;
  }

  constructor(
    private fb: FormBuilder,
    private acessoService: AcessoService,
    private sessaoService: SessaoInfoService,
    private moduloService: ModuloService,
    private notificacaoService: NotificacaoService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      codigo: ['', Validators.required],
      descricao: ['', Validators.required],
      modulos: [[], Validators.required]
    });

    this.carregarDadosSessao();

    this.dataSource.filterPredicate = (modulo, filtro) => {
      const termo = filtro.trim().toLowerCase();
      return modulo.codigo.toLowerCase().includes(termo)
        || modulo.descricao.toLowerCase().includes(termo);
    };

    this.buscaModuloControl.valueChanges.subscribe(termo => {
      this.dataSource.filter = (termo ?? '').trim().toLowerCase();
      if (this.dataSource.paginator) {
        this.dataSource.paginator.firstPage();
      }
    });

    this.carregarModulos();
  }

  private carregarDadosSessao(): void {
    this.sessaoService.obterDadosUsuario().subscribe({
      next: (dados) => {
        if (dados) {
          this.usuarioCodigo = dados.codigoDoUsuario ?? this.sessaoService.obterUsuarioCodigo() ?? '';
          this.workspaceCodigo = dados.workspace ?? '';
          this.usuarioNome = dados.nomeDoUsuario;
        }
      },
      error: (erro) => {
        const mensagens = this.notificacaoService.normalizarMensagens(erro?.error);
        if (mensagens.length) {
          this.notificacaoService.exibirMensagens(mensagens);
        }
      }
    });
  }

  private carregarModulos(): void {
    this.carregandoModulos = true;
    this.moduloService.obterTodos()
      .pipe(finalize(() => this.carregandoModulos = false))
      .subscribe({
        next: (modulos) => {
          this.todosModulos = modulos;
          this.dataSource.data = modulos;
        },
        error: (erro) => {
          const mensagens = this.notificacaoService.normalizarMensagens(erro?.error);
          if (mensagens.length) {
            this.notificacaoService.exibirMensagens(mensagens);
          } else {
            this.notificacaoService.exibirMensagem('Erro ao carregar a lista de módulos.', undefined, 6000);
          }
        }
      });
  }

  get mensagemEstado(): string {
    const termo = (this.buscaModuloControl.value ?? '').trim();
    if (termo && this.dataSource.filteredData.length === 0) {
      return 'Nenhum módulo encontrado para a busca informada.';
    }
    if (!this.todosModulos.length) {
      return 'Nenhum módulo disponível para montar o perfil.';
    }
    return '';
  }

  estaSelecionado(codigo: string): boolean {
    const selecionados = this.form.get('modulos')?.value || [];
    return selecionados.includes(codigo);
  }

  onToggleModulo(codigo: string, selecionado: boolean): void {
    const control = this.form.get('modulos');
    const selecionados = control?.value || [];

    if (selecionado && !selecionados.includes(codigo)) {
      control?.setValue([...selecionados, codigo]);
    } else if (!selecionado) {
      control?.setValue(selecionados.filter((c: string) => c !== codigo));
    }
  }

  salvar(): void {
    if (this.form.invalid || this.salvando) {
      return;
    }

    this.salvando = true;

    const payload = {
      codigo: this.form.value.codigo,
      descricao: this.form.value.descricao,
      modulos: (this.form.value.modulos as string[]).map(cod => ({ codigo: cod }))
    };

    this.acessoService.configurarPerfilInicial(payload)
      .pipe(finalize(() => this.salvando = false))
      .subscribe({
        next: () => {
          // Limpa a sessão local para forçar a sincronização de permissões com o backend
          this.sessaoService.clearInfo();
          this.router.navigate(['/login']);
        },
        error: (erro) => {
          const mensagens = this.notificacaoService.normalizarMensagens(erro?.error);
          if (mensagens.length) {
            this.notificacaoService.exibirMensagens(mensagens);
          } else {
            this.notificacaoService.exibirMensagem('Não foi possível salvar o perfil inicial.', undefined, 6000);
          }
        }
      });
  }
}