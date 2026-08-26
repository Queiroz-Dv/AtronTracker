import {
  ProcessamentoProdutoFiltros,
  processamentoCorrespondeAosFiltros
} from './processamento-produto-filtros';
import { ProcessamentoProduto, ProcessamentoProdutoStatus } from './processamento-produto.model';

describe('Filtros de processamento de Produto', () => {
  const processamento = criarProcessamento();

  it('localiza por id, código e descrição', () => {
    expect(corresponde({ texto: '37', status: null })).toBeTrue();
    expect(corresponde({ texto: 'note', status: null })).toBeTrue();
    expect(corresponde({ texto: 'patrimonial', status: null })).toBeTrue();
  });

  it('restringe pelo status selecionado', () => {
    expect(corresponde({ texto: '', status: ProcessamentoProdutoStatus.Concluido })).toBeTrue();
    expect(corresponde({ texto: '', status: ProcessamentoProdutoStatus.Falha })).toBeFalse();
  });

  function corresponde(filtros: ProcessamentoProdutoFiltros): boolean {
    return processamentoCorrespondeAosFiltros(processamento, JSON.stringify(filtros));
  }

  function criarProcessamento(): ProcessamentoProduto {
    return {
      id: 37, status: ProcessamentoProdutoStatus.Concluido, codigoBase: 'NOTE',
      quantidadeSolicitada: 100, quantidadeProcessada: 100,
      descricao: 'Notebook patrimonial', descricaoComplementar: null,
      dataAquisicao: '2026-08-26', precoUnitario: 3500, categoriaCodigos: ['INF'],
      loteProdutoId: 4, loteProdutoCodigo: 'NOTE_2026', erro: null
    };
  }
});
