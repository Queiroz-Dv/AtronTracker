export class TarefaViewData {
  id: number;
  solicitacaoId: number | null;
  exigeAprovacaoParaObter: boolean;
  nomeDoUsuario: string;
  cargoDescricao: string;
  departamentoDescricao: string;

  titulo: string;
  descricaoDoEstadoDaTarefa: string;
  dataInicial: Date;
  dataFinal: Date;
}
