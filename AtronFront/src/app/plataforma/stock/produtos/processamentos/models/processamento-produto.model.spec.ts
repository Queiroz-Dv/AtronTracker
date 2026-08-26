import {
  ProcessamentoProduto,
  ProcessamentoProdutoStatus,
  calcularProgressoProcessamento,
  descreverStatusProcessamento,
  processamentoFinalizado
} from './processamento-produto.model';

describe('ProcessamentoProduto', () => {
  it('descreve os quatro estados do backend', () => {
    expect(descreverStatusProcessamento(ProcessamentoProdutoStatus.Pendente)).toBe('Pendente');
    expect(descreverStatusProcessamento(ProcessamentoProdutoStatus.EmExecucao)).toBe('Em execução');
    expect(descreverStatusProcessamento(ProcessamentoProdutoStatus.Concluido)).toBe('Concluído');
    expect(descreverStatusProcessamento(ProcessamentoProdutoStatus.Falha)).toBe('Falha');
  });

  it('encerra o polling somente para conclusão ou falha', () => {
    expect(processamentoFinalizado(ProcessamentoProdutoStatus.Pendente)).toBeFalse();
    expect(processamentoFinalizado(ProcessamentoProdutoStatus.EmExecucao)).toBeFalse();
    expect(processamentoFinalizado(ProcessamentoProdutoStatus.Concluido)).toBeTrue();
    expect(processamentoFinalizado(ProcessamentoProdutoStatus.Falha)).toBeTrue();
  });

  it('calcula o progresso sem ultrapassar cem por cento', () => {
    const processamento = criarProcessamento();
    processamento.quantidadeProcessada = 4;
    expect(calcularProgressoProcessamento(processamento)).toBe(40);
    processamento.quantidadeProcessada = 12;
    expect(calcularProgressoProcessamento(processamento)).toBe(100);
  });

  function criarProcessamento(): ProcessamentoProduto {
    return {
      id: 7, status: ProcessamentoProdutoStatus.EmExecucao, codigoBase: 'NOTE',
      quantidadeSolicitada: 10, quantidadeProcessada: 0, descricao: 'Notebook',
      descricaoComplementar: null, dataAquisicao: '2026-08-26', precoUnitario: 3000,
      categoriaCodigos: [], loteProdutoId: null, loteProdutoCodigo: null, erro: null
    };
  }
});
