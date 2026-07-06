export class DestinoInicialTarefa {
  constructor(public id: number, public descricao: string) { }

  static readonly Usuario = 1;
  static readonly DepartamentoCargo = 2;
  static readonly Equipe = 3;

  static getDestinos(): DestinoInicialTarefa[] {
    return [
      { id: DestinoInicialTarefa.Usuario, descricao: 'Usuario' },
      { id: DestinoInicialTarefa.DepartamentoCargo, descricao: 'Departamento/Cargo' },
      { id: DestinoInicialTarefa.Equipe, descricao: 'Equipe' }
    ];
  }
}
