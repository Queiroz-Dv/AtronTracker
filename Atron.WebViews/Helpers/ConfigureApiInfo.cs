using Shared.Extensions;

namespace Atron.WebViews.Helpers
{
    public class ConfigureApiInfo
    {
        public static string Url { get; set; }

        public static string Rota { get; set; }

        public static string ApiActionName { get; set; }

        public static string ApiControllerName { get; set; }

        public static string ApiControllerAction { get; set; }

        public static string Parameter { get; set; }


        public static void BuildMainRoute()
        {
            Rota = $"{AppSettings.RotaDeAcesso.Protocolo}{AppSettings.RotaDeAcesso.Url}/";
        }

        public static void BuildModuleUrl()
        {
            Url = Parameter.IsNullOrEmpty() ? $"{Rota}{ApiControllerName}/{ApiActionName}" :
                                              $"{Rota}{ApiControllerName}/{ApiActionName}{Parameter}";
        }

        public static string GetUrlPath()
        {
            return Url;
        }
    }
}