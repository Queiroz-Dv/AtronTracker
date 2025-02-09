using Shared.Extensions;

namespace Atron.WebViews.Helpers
{
    public record ConfigureApiInfo
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

        public static void ConfigureAndBuildRoute(string apiControllerName, string apiActionName, string parameter)
        {
            ApiControllerName = apiControllerName;
            ApiActionName = apiActionName;
            Parameter = parameter;
            BuildMainRoute();
            BuildModuleUrl();
        }
    }

    public static class ConfigureApiExtensions
    {
        public static string ConfigureAndGetUrlPath(string apiControllerName, string apiActionName, string parameter)
        {
            ConfigureApiInfo.ConfigureAndBuildRoute(apiControllerName, apiActionName, parameter);
            return ConfigureApiInfo.Url;
        }
    }
}