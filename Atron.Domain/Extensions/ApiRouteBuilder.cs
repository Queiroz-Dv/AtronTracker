namespace Atron.Domain.Extensions
{
    public class ApiRouteBuilder
    {
        public static string BuildRoute(string protocolo, string url)
        {
            return $"{protocolo}{url}/";
        }

        public static string BuildUrl(string route, 
            string nomeControlador, 
            string nomeDaAction = "",
            string parameter = "")
        {
            return string.IsNullOrEmpty(parameter) ? $"{route}{nomeControlador}/{nomeDaAction}" :
                                                     $"{route}{nomeControlador}/{nomeDaAction}/{parameter}";
        }
    }
}