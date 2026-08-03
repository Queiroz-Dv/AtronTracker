import { MODULOS_CONFIG } from '../../shared/utils/modulo-functions.util';

export class Modulo {
  code: string;
  title: string;
  icon: string;
  description: string;
  route: string;

  static getModulos(): Modulo[] {
    return Object.entries(MODULOS_CONFIG).map(([code, config]) => ({
      code,
      title: config.titulo,
      icon: config.icone,
      description: config.descricao,
      route: config.rota,
    }));
  }
}