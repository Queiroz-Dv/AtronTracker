export class DestinoInicialTarefa {
  constructor(
    public id: number, 
    public descricao: string, 
    public valorEnvio: string
  ) { }

  static readonly Usuario = 1;
  static readonly DepartamentoCargo = 2;
  static readonly Equipe = 3;

  static getDestinos(): DestinoInicialTarefa[] {
    return [
      { id: DestinoInicialTarefa.Usuario, descricao: 'Usuário', valorEnvio: 'Usuario' },
      { id: DestinoInicialTarefa.DepartamentoCargo, descricao: 'Departamento/Cargo', valorEnvio: 'Departamento Cargo' },
      { id: DestinoInicialTarefa.Equipe, descricao: 'Equipe', valorEnvio: 'Equipe' }
    ];
  }

  static obterValorEnvioPorId(id: number): string {
    const destino = this.getDestinos().find(d => d.id === id);
    return destino ? destino.valorEnvio : 'Usuario';
  }

  static obterIdPorValorEnvio(valor: string): number {
    const destino = this.getDestinos().find(d => d.valorEnvio === valor || d.descricao === valor);
    return destino ? destino.id : DestinoInicialTarefa.Usuario;
  }
}