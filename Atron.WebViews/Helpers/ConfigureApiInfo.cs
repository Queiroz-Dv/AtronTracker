using Shared.Extensions;

namespace Atron.WebViews.Helpers
{
    public record ConfigureApiInfo
    {
        public static string Url { get; set; }
        public static string Rota { get; set; }
        public static string ApiActionName { get; set; }
        public static string ApiControllerName { get; set; }

        public static void BuildModuleUrl()
        {
            Rota = $"{AppSettings.RotaDeAcesso.Protocolo}{AppSettings.RotaDeAcesso.Url}";
            Url = $"{Rota}/{ApiControllerName}/{ApiActionName}";
        }

        public static void ConfigureAndBuildRoute(string apiControllerName, string apiActionName, string parameter)
        {
            ApiControllerName = apiControllerName;
            ApiActionName = apiActionName;
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