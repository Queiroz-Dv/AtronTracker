export class TarefaViewData {
  id: number;
  solicitacaoId: number | null;
  identificador: number | null;
  destinoDescricao: string;
  exigeAprovacaoParaObter: boolean;
  nomeDoSolicitante: string;
  nomeDoUsuario: string;
  cargoDescricao: string;
  departamentoDescricao: string;

  titulo: string;
  descricaoDoEstadoDaTarefa: string;
  dataInicial: Date;
  dataFinal: Date;
}
