import {
  ProcessamentoProduto,
  ProcessamentoProdutoStatus,
  descreverStatusProcessamento
} from './processamento-produto.model';

export interface ProcessamentoProdutoFiltros {
  texto: string;
  status: ProcessamentoProdutoStatus | null;
}

export const FILTROS_PROCESSAMENTO_INICIAIS: ProcessamentoProdutoFiltros = {
  texto: '',
  status: null
};

export function processamentoCorrespondeAosFiltros(
  processamento: ProcessamentoProduto,
  filtroSerializado: string
): boolean {
  const filtros = JSON.parse(filtroSerializado) as ProcessamentoProdutoFiltros;
  const texto = filtros.texto.toLowerCase();
  const correspondeAoTexto = !texto
    || processamento.codigoBase.toLowerCase().includes(texto)
    || processamento.descricao.toLowerCase().includes(texto)
    || String(processamento.id).includes(texto)
    || descreverStatusProcessamento(processamento.status).toLowerCase().includes(texto);
  return correspondeAoTexto
    && (filtros.status === null || processamento.status === filtros.status);
}
