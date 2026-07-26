import { AfterViewInit, Component, OnDestroy, inject, ViewChild } from '@angular/core';
import { MatTable, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { combineLatest, Subscription } from 'rxjs';
import { TarefaService } from '../../services/tarefa.service';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../../../shared/modules/shared.module';
import { TarefaViewData } from '../../models/tarefa-view-data.model';
import { formatLabel } from '../../../../shared/utils/formatar-label.util';
import { BotaoVoltarComponent } from "../../../../core/layout/botao-voltar/botao-voltar.component";
import { MatSort } from '@angular/material/sort';
import { TarefaResponse } from '../../models/response/tarefa-response.model';
import { SolicitacaoObtencaoTarefaResponse } from '../../models/response/solicitacao-obtencao-tarefa-response.model';
import { NotificacaoInternaService } from '../../../notificacoes/services/notificacao-interna.service';

type TarefaVisao = 'meuQuadro' | 'equipe' | 'disponiveis' | 'solicitacoes';

@Component({
  selector: 'c-tarefa-view',
  templateUrl: './tarefa-view.component.html',
  imports: [SharedModule, ReactiveFormsModule, BotaoVoltarComponent],
})

export class TarefaViewComponent implements AfterViewInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  dataSource: MatTableDataSource<TarefaViewData>;
  visaoAtual: TarefaVisao = 'meuQuadro';
  possuiResponsabilidadeGestao = false;
  private queryParamsSubscription?: Subscription;

  route = inject(ActivatedRoute);
  colunas = [
    'usuario',
    'cargo',
    'departamento',
    'titulo',
    'dataInicial',
    'dataFinal',
    'estado',
    'acoes'];

  constructor(
    private service: TarefaService,
    private notificacaoInternaService: NotificacaoInternaService,
    public router: Router
  ) { }

  ngAfterViewInit() {
    this.queryParamsSubscription = combineLatest([
      this.service.obterAcesso(),
      this.route.queryParamMap
    ]).subscribe(([acesso, params]) => {
      this.possuiResponsabilidadeGestao = acesso.possuiResponsabilidadeGestao;
      this.carregarVisao(this.normalizarVisao(params.get('visao')));
    });
  }

  ngOnDestroy(): void {
    this.queryParamsSubscription?.unsubscribe();
  }

  carregar() {
    this.carregarVisao(this.visaoAtual);
  }

  selecionarVisao(visao: TarefaVisao): void {
    if (this.visaoAtual === visao) {
      this.carregarVisao(visao);
      return;
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { visao },
      queryParamsHandling: 'merge'
    });
  }

  carregarMeuQuadro() {
    this.visaoAtual = 'meuQuadro';
    this.service.obterMeuQuadro().subscribe(tarefas => this.atualizarTabela(tarefas));
  }

  carregarEquipe() {
    this.visaoAtual = 'equipe';
    this.service.obterEquipe().subscribe(tarefas => this.atualizarTabela(tarefas));
  }

  carregarDisponiveis() {
    this.visaoAtual = 'disponiveis';
    this.service.obterDisponiveis().subscribe(tarefas => this.atualizarTabela(tarefas));
  }

  carregarSolicitacoes() {
    this.visaoAtual = 'solicitacoes';
    this.service.obterSolicitacoes().subscribe(solicitacoes => this.atualizarTabelaSolicitacoes(solicitacoes));
  }

  editar(id: number): void {
    this.router.navigate(['atron/tarefas/editar', id]);
  }

  assumir(id: number): void {
    this.service.assumir(id).subscribe(() => {
      this.atualizarNotificacoes();
      this.carregarMeuQuadro();
    });
  }

  obter(trf: TarefaViewData): void {
    if (!this.possuiResponsabilidadeGestao || trf.exigeAprovacaoParaObter) {
      this.service.solicitarObtencao(trf.id).subscribe(() => {
        this.atualizarNotificacoes();
        this.carregarDisponiveis();
      });
      return;
    }

    this.assumir(trf.id);
  }

  aprovar(solicitacaoId: number | null): void {
    if (!solicitacaoId) return;
    this.service.aprovarSolicitacao(solicitacaoId).subscribe(() => {
      this.atualizarNotificacoes();
      this.carregarSolicitacoes();
    });
  }

  recusar(solicitacaoId: number | null): void {
    if (!solicitacaoId) return;
    this.service.recusarSolicitacao(solicitacaoId).subscribe(() => {
      this.atualizarNotificacoes();
      this.carregarSolicitacoes();
    });
  }

  excluir(id: number): void {
    if (confirm('Deseja realmente excluir?')) {
      this.service.deletar(id).subscribe(() => this.carregar());
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  formatDate(arg0: any) {
    const full_date = new Date(arg0).toLocaleDateString("pt-BR");
    return full_date;
  }

  private carregarVisao(visao: TarefaVisao): void {
    if (visao === 'solicitacoes') {
      this.carregarSolicitacoes();
      return;
    }

    if (visao === 'disponiveis') {
      this.carregarDisponiveis();
      return;
    }

    if (visao === 'equipe') {
      this.carregarEquipe();
      return;
    }

    this.carregarMeuQuadro();
  }

  private normalizarVisao(visao: string | null): TarefaVisao {
    if (visao === 'disponiveis') {
      return visao;
    }

    if (this.possuiResponsabilidadeGestao && (visao === 'equipe' || visao === 'solicitacoes')) {
      return visao;
    }

    return 'meuQuadro';
  }

  private atualizarNotificacoes(): void {
    this.notificacaoInternaService.carregar().subscribe({
      error: () => undefined
    });
  }

  private atualizarTabela(tarefas: TarefaResponse[]): void {
    const tarefaViewData: TarefaViewData[] = tarefas.map(trf => ({
      id: trf.id,
      solicitacaoId: null as number | null,
      exigeAprovacaoParaObter: trf.exigeAprovacaoParaObter,
      nomeDoUsuario: trf.usuario
        ? `${trf.usuario.codigo} - ${trf.usuario.nome} ${trf.usuario.sobrenome}`
        : '-',
      cargoDescricao: trf.cargo
        ? formatLabel(trf.cargo.codigo, trf.cargo.descricao)
        : formatLabel(trf.usuario?.cargo?.codigo, trf.usuario?.cargo?.descricao),
      departamentoDescricao: trf.departamento
        ? formatLabel(trf.departamento.codigo, trf.departamento.descricao)
        : formatLabel(trf.usuario?.departamento?.codigo, trf.usuario?.departamento?.descricao),
      descricaoDoEstadoDaTarefa: trf.estadoDaTarefa.descricao,
      titulo: trf.titulo,
      dataInicial: trf.dataInicial,
      dataFinal: trf.dataFinal
    }));

    this.dataSource = new MatTableDataSource(tarefaViewData);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  private atualizarTabelaSolicitacoes(solicitacoes: SolicitacaoObtencaoTarefaResponse[]): void {
    const tarefas = solicitacoes.map(solicitacao => ({
      ...solicitacao.tarefa,
      solicitacaoId: solicitacao.id
    }));

    const tarefaViewData: TarefaViewData[] = tarefas.map(trf => ({
      id: trf.id,
      solicitacaoId: trf.solicitacaoId ?? null,
      exigeAprovacaoParaObter: trf.exigeAprovacaoParaObter,
      nomeDoUsuario: trf.usuario
        ? `${trf.usuario.codigo} - ${trf.usuario.nome} ${trf.usuario.sobrenome}`
        : '-',
      cargoDescricao: trf.cargo
        ? formatLabel(trf.cargo.codigo, trf.cargo.descricao)
        : formatLabel(trf.usuario?.cargo?.codigo, trf.usuario?.cargo?.descricao),
      departamentoDescricao: trf.departamento
        ? formatLabel(trf.departamento.codigo, trf.departamento.descricao)
        : formatLabel(trf.usuario?.departamento?.codigo, trf.usuario?.departamento?.descricao),
      descricaoDoEstadoDaTarefa: trf.estadoDaTarefa.descricao,
      titulo: trf.titulo,
      dataInicial: trf.dataInicial,
      dataFinal: trf.dataFinal
    }));

    this.dataSource = new MatTableDataSource(tarefaViewData);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
}
