export class Modulo {
  code: string; // código único para o card
  title: string;
  icon: string;        // nome do ícone Material ou classe FontAwesome
  description: string;
  route: string;       // rota Angular para navegação 

  static getModulos(): Modulo[] {
    return [{
      code: 'DPT',
      title: 'Departamentos',
      icon: 'business', // ou 'fa-solid fa-building'
      description: 'Gerencie os departamentos da empresa.',
      route: '/atron/departamentos'
    },
    {
      code: 'CRG',
      title: 'Cargos',
      icon: 'work', // ou 'fa-solid fa-briefcase'
      description: 'Gerencie os cargos da empresa.',
      route: '/atron/cargos',
    },
    {
      code: 'USR',
      title: 'Usuários',
      icon: 'group', // ou 'fa-solid fa-users'
      description: 'Gerencie os colaboradores da empresa.',
      route: '/atron/usuarios',
    },
    {
      code: 'TAR',
      title: 'Tarefas',
      icon: 'checklist', // ou 'fa-solid fa-list-check'
      description: 'Gerencie as tarefas da empresa.',
      route: '/atron/tarefas',
    },
    {
      code: 'SAL',
      title: 'Salários',
      icon: 'attach_money', // ou 'fa-solid fa-money-bill'
      description: 'Gerencie o salário da empresa.',
      route: '/atron/salarios',
    },
    {
      code: 'PAC',
      title: 'Políticas e Acessos',
      icon: 'tune', // ou 'fa-solid fa-sliders'
      description: 'Gerencie as políticas de acesso do sistema.',
      route: '/atron/politicas',
    }]
  }
}