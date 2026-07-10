export type ModuloConfig = {
  titulo: string;
  icone: string;
  rota: string;
  descricao: string;
};

export const MODULOS_CONFIG: Record<string, ModuloConfig> = {
  DPT: {
    titulo: 'Departamentos',
    icone: 'business',
    rota: '/atron/departamentos',
    descricao: 'Gerencie os departamentos da empresa.'
  },
  CRG: {
    titulo: 'Cargos',
    icone: 'work',
    rota: '/atron/cargos',
    descricao: 'Gerencie os cargos da empresa.'
  },
  PLC: {
    titulo: 'Planejamento de Custos',
    icone: 'account_balance',
    rota: '/atron/planejamento-custos',
    descricao: 'Gerencie planejamentos anuais de custo por departamento.'
  },
  USR: {
    titulo: 'Usuários',
    icone: 'group',
    rota: '/atron/usuarios',
    descricao: 'Gerencie os colaboradores da empresa.'
  },
  TAR: {
    titulo: 'Tarefas',
    icone: 'checklist',
    rota: '/atron/tarefas',
    descricao: 'Gerencie as tarefas da empresa.'
  },
  PERF: {
    titulo: 'Perfil de acesso',
    icone: 'tune',
    rota: '/atron/perfil-de-acesso',
    descricao: 'Gerencie os perfis de acesso do sistema.'
  },
  RPERFUSR: {
    titulo: 'Relacionamento de perfil e usuários',
    icone: 'supervisor_account',
    rota: '/atron/perfil-de-acesso/relacionamento-perfil-usuario/novo',
    descricao: 'Configure os acessos do usuário de acordo com os perfis do sistema.'
  }
};

function getModuloConfig(codigo: string): ModuloConfig {
  return MODULOS_CONFIG[codigo] || {
    titulo: codigo,
    icone: 'default-icon',
    rota: '/atron/default',
    descricao: 'Módulo não reconhecido.'
  };
}

export class ModuloItem {
  titulo: string;
  icone: string;
  descricao: string;
  rota: string;

  constructor(public codigo: string) {
    const config = getModuloConfig(codigo);
    this.titulo = config.titulo;
    this.icone = config.icone;
    this.descricao = config.descricao;
    this.rota = config.rota;
  }
}
