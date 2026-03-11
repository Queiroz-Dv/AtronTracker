type ModuloConfig = {
  icone: string;
  rota: string;
  descricao: string;
};

const MODULOS_CONFIG: Record<string, ModuloConfig> = {
  DPT: {
    icone: 'business',
    rota: '/atron/departamentos',
    descricao: 'Gerencie os departamentos da empresa.'
  },
  CRG: {
    icone: 'work',
    rota: '/atron/cargos',
    descricao: 'Gerencie os cargos da empresa.'
  },
  USR: {
    icone: 'group',
    rota: '/atron/usuarios',
    descricao: 'Gerencie os colaboradores da empresa.'
  },
  TRF: {
    icone: 'checklist',
    rota: '/atron/tarefas',
    descricao: 'Gerencie as tarefas da empresa.'
  },
  SAL: {
    icone: 'attach_money',
    rota: '/atron/salarios',
    descricao: 'Gerencie o salário da empresa.'
  },
  PRF: {
    icone: 'tune',
    rota: '/atron/perfil-de-acesso',
    descricao: 'Gerencie os perfis de acesso do sistema.'
  },
  RPERFUSR: {
    icone: 'supervisor_account',
    rota: '/atron/perfil-de-acesso/relacionamento-perfil-usuario/novo',
    descricao: 'Configure os acessos do usuário de acordo com os perfis do sistema.'
  }
};

function getModuloConfig(codigo: string): ModuloConfig {
  return MODULOS_CONFIG[codigo] || {
    icone: 'default-icon',
    rota: '/atron/default',
    descricao: 'Módulo não reconhecido.'
  };
}

export class ModuloItem {
  icone: string;
  descricao: string;
  rota: string;

  constructor(public codigo: string) {
    const config = getModuloConfig(codigo);
    this.icone = config.icone;
    this.descricao = config.descricao;
    this.rota = config.rota;
  }
}
