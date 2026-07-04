import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';

export interface Mensagem {
  descricao: string;
  nivel: Nivel; 
}

export enum Nivel {
  Mensagem = 'Mensagem',
  Sucesso = 'Sucesso',
  Aviso = 'Aviso',
  Error = 'Error',
}

interface NivelConfig {
  prioridade: number; 
  duracao?: number;
}

@Injectable({ providedIn: 'root' })

export class NotificacaoService {
  
// Funciona como um dicionário no C#
  private configPorNivel: Map<Nivel, NivelConfig> = new Map([
    [Nivel.Error,    { prioridade: 1, duracao: 6000 }],
    [Nivel.Sucesso,  { prioridade: 2, duracao: 5000 }],
    [Nivel.Aviso,    { prioridade: 3, duracao: 4000 }],
    [Nivel.Mensagem, { prioridade: 4 }], 
  ]);

  constructor(private snackBar: MatSnackBar) { }

  exibirMensagem(mensagem: string, nivel?: Nivel, duracao?: number): void {
    const config = new MatSnackBarConfig();
    config.panelClass = this.obterClassePainel(nivel);
    config.duration = duracao ?? this.configPorNivel.get(nivel)?.duracao;  
    this.snackBar.open(mensagem, 'Fechar', config);
  }

  exibirMensagens(mensagens: unknown): void {
    const mensagensNormalizadas = this.normalizarMensagens(mensagens);

    if (!mensagensNormalizadas.length) {
      return;
    }

    const mensagemParaExibir = this.obterMensagemPrioritaria(mensagensNormalizadas);
    if (mensagemParaExibir) {
      this.exibirMensagem(mensagemParaExibir.descricao, mensagemParaExibir.nivel);
    }
  }

  temMensagemDeErro(mensagens: unknown): boolean {
    return this.normalizarMensagens(mensagens).some(msg => msg.nivel === Nivel.Error);
  }

  normalizarMensagens(mensagens: unknown): Mensagem[] {
    if (!mensagens) {
      return [];
    }

    const entrada = this.extrairEntradaDeMensagens(mensagens);
    if (!Array.isArray(entrada)) {
      return [];
    }

    return entrada
      .map(msg => this.normalizarMensagem(msg))
      .filter((msg): msg is Mensagem => !!msg);
  }

  private obterMensagemPrioritaria(mensagens: Mensagem[]): Mensagem | null {
    let mensagemParaExibir: Mensagem | null = null;
    let maiorPrioridade = Infinity;

    for (const msg of mensagens) {
      const config = this.configPorNivel.get(msg.nivel);
      if (config && config.prioridade < maiorPrioridade) {
        maiorPrioridade = config.prioridade;
        mensagemParaExibir = msg;
      }
    }

    return mensagemParaExibir;
  }

  private extrairEntradaDeMensagens(mensagens: unknown): unknown {
    if (Array.isArray(mensagens)) {
      return mensagens;
    }

    if (typeof mensagens !== 'object') {
      return null;
    }

    const objeto = mensagens as Record<string, unknown>;
    if ((objeto['descricao'] ?? objeto['Descricao']) && (objeto['nivel'] ?? objeto['Nivel'])) {
      return [objeto];
    }

    return objeto['mensagensApi']
      ?? objeto['mensagens']
      ?? objeto['messages']
      ?? objeto['Messages']
      ?? null;
  }

  private normalizarMensagem(mensagem: unknown): Mensagem | null {
    if (!mensagem || typeof mensagem !== 'object') {
      return null;
    }

    const objeto = mensagem as Record<string, unknown>;
    const descricao = objeto['descricao'] ?? objeto['Descricao'];
    const nivel = objeto['nivel'] ?? objeto['Nivel'];

    if (typeof descricao !== 'string' || typeof nivel !== 'string') {
      return null;
    }

    if (!Object.values(Nivel).includes(nivel as Nivel)) {
      return null;
    }

    return {
      descricao,
      nivel: nivel as Nivel
    };
  }

  private obterClassePainel(nivel: Nivel): string[] {
    switch (nivel) {
      case Nivel.Sucesso: return ['snackbar-sucesso'];
      case Nivel.Error: return ['snackbar-erro'];
      case Nivel.Aviso: return ['snackbar-aviso'];
      case Nivel.Mensagem: return ['snackbar-info'];      
      default: return [];
    }
  }
}
