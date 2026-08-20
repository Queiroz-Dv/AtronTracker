using System.ComponentModel;

namespace Domain.Enums
{
    public enum TipoMovimentacaoTarefa
    {
        [Description("Criação")]
        Criacao = 1,
        [Description("Atualização")]
        Atualizacao = 2,
        [Description("Obtenção")]
        Obtencao = 3,
        [Description("Solicitação de Obtenção")]
        SolicitacaoObtencao = 4,
        [Description("Aprovação de Obtenção")]
        AprovacaoObtencao = 5,
        [Description("Recusa de Obtenção")]
        RecusaObtencao = 6
    }
}