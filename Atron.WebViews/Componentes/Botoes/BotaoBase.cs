using Atron.WebViews.Componentes.Botoes.Enums;

namespace Atron.WebViews.Componentes.Botoes
{
    public class BotaoBase
    {
        public BotaoBase(
            string nomeDaController = "", 
            string nomeDaAction = "", 
            string nomeDoBotao = "", 
            string cssClass = "",
            string icone = "")
        {
            NomeDaController = nomeDaController;
            NomeDaAction = nomeDaAction;
            NomeDoBotao = nomeDoBotao;
            CssClass = cssClass;
            Icone = icone;
        }

        public TipoBotao TipoBotao { get; set; }
        public string NomeDaController { get; private set; }
        public string NomeDaAction { get; private set; }
        public string NomeDoBotao { get; private set; }
        public string CssClass { get; private set; }
        public string Icone { get; private set; }
    }
}