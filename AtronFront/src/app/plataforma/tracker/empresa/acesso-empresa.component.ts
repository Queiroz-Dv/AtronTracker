import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AcessoService } from '../../../core/services/acesso.service';
import { ContextoEmpresa, EmpresaBusca, EmpresaCadastro, EmpresaContextoService, SolicitacaoEmpresa } from '../../../core/services/empresa-contexto.service';

@Component({
  standalone: true,
  selector: 'c-acesso-empresa',
  imports: [FormsModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule],
  templateUrl: './acesso-empresa.component.html',
  styleUrl: './acesso-empresa.component.css'
})
export class AcessoEmpresaComponent implements OnInit {
  contexto: ContextoEmpresa | null = null;
  carregando = false;
  saindo = false;
  erro = '';
  mensagem = '';
  associacao: SolicitacaoEmpresa | null = null;
  termoBusca = '';
  empresas: EmpresaBusca[] = [];
  empresaSelecionada: EmpresaBusca | null = null;
  cadastro: EmpresaCadastro = {
    codigo: '', nomeFantasia: '', endereco: { logradouro: '' }, numero: '', email: ''
  };

  constructor(
    private empresa: EmpresaContextoService,
    private acesso: AcessoService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.verificarAcesso();
  }

  verificarAcesso(): void {
    this.carregando = true;
    this.erro = '';
    this.mensagem = '';
    this.contexto = null;
    this.associacao = null;
    this.empresa.obter().pipe(finalize(() => this.carregando = false)).subscribe({
      next: contexto => {
        this.contexto = contexto;
        if (contexto?.empresaId === null) this.carregarAssociacao();
        if (contexto?.acessoPermitido === true) {
          void this.router.navigate(['/atron/dashboard']);
        }
      },
      error: () => {
        this.erro = 'Não foi possível consultar sua associação. Tente novamente.';
      }
    });
  }

  carregarAssociacao(): void {
    this.empresa.obterAssociacao().subscribe({
      next: associacao => this.associacao = associacao,
      error: () => this.associacao = null
    });
  }

  buscar(): void {
    this.erro = '';
    this.empresa.buscar(this.termoBusca).subscribe({
      next: empresas => this.empresas = empresas,
      error: () => this.erro = 'Não foi possível consultar as empresas.'
    });
  }

  selecionar(empresa: EmpresaBusca): void {
    this.empresaSelecionada = empresa;
  }

  solicitarAssociacao(): void {
    if (!this.empresaSelecionada) return;
    this.erro = '';
    this.empresa.solicitar(this.empresaSelecionada.id).subscribe({
      next: () => this.mensagem = 'Solicitação enviada. Aguarde a aprovação do responsável.',
      error: () => this.erro = 'Não foi possível enviar a solicitação.'
    });
  }

  cadastrarEmpresa(): void {
    this.erro = '';
    this.empresa.cadastrar(this.cadastro).subscribe({
      next: () => this.verificarAcesso(),
      error: () => this.erro = 'Não foi possível cadastrar a empresa.'
    });
  }

  sair(): void {
    this.saindo = true;
    this.acesso.logout().pipe(finalize(() => this.saindo = false)).subscribe(() => {
      void this.router.navigate(['/login']);
    });
  }
}
