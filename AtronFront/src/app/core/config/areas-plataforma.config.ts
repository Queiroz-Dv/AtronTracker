export interface AreaPlataformaConfig {
  readonly chave: string;
  readonly codigo: string;
  readonly titulo: string;
  readonly icone: string;
  readonly descricao: string;
  readonly descricaoRotinas: string;
  readonly codigosModulos: readonly string[];
  readonly rotuloAcesso: string;
  readonly desabilitado?: boolean;
}

export const AREAS_PLATAFORMA = {
  Tracker: {
    chave: 'tracker',
    codigo: 'ATRON_TRACKER',
    titulo: 'Atron Tracker',
    icone: 'analytics',
    descricao: 'Gestão administrativa e operacional.',
    descricaoRotinas: 'Acesse as rotinas administrativas e operacionais.',
    codigosModulos: ['DPT', 'CRG', 'PLC', 'USR', 'PERF', 'RPERFUSR', 'TAR', 'TRF'],
    rotuloAcesso: 'Acessar Tracker'
  },
  Stock: {
    chave: 'stock',
    codigo: 'ATRON_STOCK',
    titulo: 'Atron Stock',
    icone: 'inventory_2',
    descricao: 'Controle de estoque, patrimônio e rotinas comerciais.',
    descricaoRotinas: 'Acesse as rotinas de estoque, produtos e suprimentos.',
    codigosModulos: ['EST', 'STK', 'PRD', 'PROD', 'CAT', 'CLI', 'FOR', 'FRN', 'PAT', 'VEN', 'VND'],
    rotuloAcesso: 'Acessar Stock'
  },
  Sales: {
    chave: 'sales',
    codigo: 'ATRON_SALES',
    titulo: 'Atron Sales',
    icone: 'payments',
    descricao: 'Gestão financeira e comercial.',
    descricaoRotinas: 'Acesse as rotinas financeiras e comerciais.',
    codigosModulos: ['SAL', 'SALES', 'VEN', 'VND', 'FIN', 'CMP', 'COM'],
    rotuloAcesso: 'Acessar Sales',
    desabilitado: true
  }
} as const satisfies Record<string, AreaPlataformaConfig>;

export const LISTA_AREAS_PLATAFORMA: readonly AreaPlataformaConfig[] =
  Object.values(AREAS_PLATAFORMA);
