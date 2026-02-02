export class EstadoTarefa {
  id: number;
  descricao: string;

  static getEstados(): EstadoTarefa[] {
    return [
      { id: 1, descricao: 'Em Atividade' },
      { id: 2, descricao: 'Pendente de aprovação' },
      { id: 3, descricao: 'Entregue' },
      { id: 4, descricao: 'Finalizada' },
      { id: 5, descricao: 'Iniciada' }
    ];
  }
}