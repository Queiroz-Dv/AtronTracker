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

  exibirMensagens(mensagens: Mensagem[] | null): void {
    if (!mensagens || mensagens.length === 0) {
      return;
    }

    let mensagemParaExibir: Mensagem | null = null;
    let maiorPrioridade = Infinity;

    for (const msg of mensagens) {
      const config = this.configPorNivel.get(msg.nivel);
      if (config && config.prioridade < maiorPrioridade) {
        maiorPrioridade = config.prioridade;
        mensagemParaExibir = msg;
      }
    }

    if (mensagemParaExibir) {        
        this.exibirMensagem(mensagemParaExibir.descricao, mensagemParaExibir.nivel);
    } 
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