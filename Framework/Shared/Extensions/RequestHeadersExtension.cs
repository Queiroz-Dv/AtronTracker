using Microsoft.AspNetCore.Http;
using Shared.Enums;

namespace Shared.Extensions
{
    public static class RequestHeadersExtension
    {
        public static bool IsRefreshing(this HttpRequest request)
        {
            return request.Headers.ContainsKey(EHeaderInfo.RefreshTokenHeader.GetDescription().ToLower());
        }

        public static string ExtrairCodigoUsuarioDoRequest(this HttpRequest request)
        {
            return request.Headers.FirstOrDefault(x => x.Key.ToUpper() == EHeaderInfo.CodigoDoUsuarioHeader.GetDescription()).Value;
        }

        public static string ExtrairCodigoUsuarioDoRequest(this IHeaderDictionary request)
        {
            return request.FirstOrDefault(x => x.Key.ToUpper() == EHeaderInfo.CodigoDoUsuarioHeader.GetDescription()).Value;
        }      
    }
}