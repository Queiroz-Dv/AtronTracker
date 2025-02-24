using System.ComponentModel;

namespace Atron.WebViews.Componentes.Botoes.Enums
{
    public enum TipoBotao
    {
        [Description("Adicionar")]
        Adicionar,
        [Description("Editar")]
        Editar,
        [Description("Excluir")]
        Excluir,
        [Description("Visualizar")]
        Visualizar,
        [Description("Salvar")]
        Salvar,
        [Description("Cancelar")]
        Cancelar,
        [Description("Pesquisar")]
        Pesquisar,
        [Description("Imprimir")]
        Imprimir,
        [Description("Exportar")]
        Exportar,
        [Description("Importar")]
        Importar,
        [Description("Fechar")]
        Fechar,
        [Description("Voltar")]
        Voltar,
        [Description("Avançar")]
        Avancar,
        [Description("Confirmar")]
        Confirmar,
        [Description("Desfazer")]
        Desfazer,
        [Description("Enviar")]
        Enviar,
        [Description("Receber")]
        Receber,
        [Description("Enviar Email")]
        EnviarEmail,
        [Description("Receber Email")]
        ReceberEmail
    }
}