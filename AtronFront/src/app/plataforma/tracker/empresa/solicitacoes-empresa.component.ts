import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';
import { EmpresaContextoService, SolicitacaoEmpresa } from '../../../core/services/empresa-contexto.service';

@Component({
  standalone: true,
  selector: 'c-solicitacoes-empresa',
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './solicitacoes-empresa.component.html',
  styleUrl: './solicitacoes-empresa.component.css'
})
export class SolicitacoesEmpresaComponent implements OnInit {
  solicitacoes: SolicitacaoEmpresa[] = [];
  carregando = false;
  processando: number | null = null;
  erro = '';
  mensagem = '';

  constructor(private empresa: EmpresaContextoService) { }

  ngOnInit(): void { this.carregar(); }

  carregar(): void {
    this.carregando = true;
    this.erro = '';
    this.empresa.solicitacoes().pipe(finalize(() => this.carregando = false)).subscribe({
      next: solicitacoes => this.solicitacoes = solicitacoes,
      error: () => this.erro = 'Você não possui permissão ou não foi possível carregar as solicitações.'
    });
  }

  decidir(solicitacao: SolicitacaoEmpresa, aprovar: boolean): void {
    this.processando = solicitacao.id;
    this.erro = '';
    this.mensagem = '';
    const operacao = aprovar
      ? this.empresa.aprovarSolicitacao(solicitacao.id)
      : this.empresa.recusarSolicitacao(solicitacao.id);

    operacao.pipe(finalize(() => this.processando = null)).subscribe({
      next: () => {
        this.mensagem = aprovar ? 'Solicitação aprovada.' : 'Solicitação recusada.';
        this.carregar();
      },
      error: () => this.erro = 'Não foi possível concluir a decisão.'
    });
  }
}
