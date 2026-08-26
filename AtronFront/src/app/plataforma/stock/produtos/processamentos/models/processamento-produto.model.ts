export enum ProcessamentoProdutoStatus {
  Pendente = 1,
  EmExecucao = 2,
  Concluido = 3,
  Falha = 4
}

export interface ProcessamentoProduto {
  id: number;
  status: ProcessamentoProdutoStatus;
  codigoBase: string;
  quantidadeSolicitada: number;
  quantidadeProcessada: number;
  descricao: string;
  descricaoComplementar: string | null;
  dataAquisicao: string;
  precoUnitario: number;
  categoriaCodigos: string[];
  loteProdutoId: number | null;
  loteProdutoCodigo: string | null;
  erro: string | null;
}

export function descreverStatusProcessamento(status: ProcessamentoProdutoStatus): string {
  const descricoes: Record<ProcessamentoProdutoStatus, string> = {
    [ProcessamentoProdutoStatus.Pendente]: 'Pendente',
    [ProcessamentoProdutoStatus.EmExecucao]: 'Em execução',
    [ProcessamentoProdutoStatus.Concluido]: 'Concluído',
    [ProcessamentoProdutoStatus.Falha]: 'Falha'
  };
  return descricoes[status] ?? 'Desconhecido';
}

export function processamentoFinalizado(status: ProcessamentoProdutoStatus): boolean {
  return status === ProcessamentoProdutoStatus.Concluido
    || status === ProcessamentoProdutoStatus.Falha;
}

export function calcularProgressoProcessamento(processamento: ProcessamentoProduto): number {
  if (processamento.quantidadeSolicitada <= 0) return 0;
  return Math.min(100, Math.round(
    processamento.quantidadeProcessada * 100 / processamento.quantidadeSolicitada
  ));
}
