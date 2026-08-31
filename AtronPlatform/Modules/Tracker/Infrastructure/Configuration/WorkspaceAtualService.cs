using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Infrastructure.Configuration;

public sealed class WorkspaceAtualService(
    IResponseCookies cookies,
    IHttpContextAccessor httpContextAccessor)
    : IWorkspaceAtualService
{
    public const string NomeCookie = "ATRON_WORKSPACE_ATUAL";

    public void Definir(int workspaceId)
    {
        cookies.Append(
            NomeCookie,
            workspaceId.ToString(CultureInfo.InvariantCulture),
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
                Path = "/"
            });
    }

    public int? ObterId()
    {
        var valor = httpContextAccessor.HttpContext?.Request.Cookies[NomeCookie];

        return int.TryParse(
            valor,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var workspaceId)
            && workspaceId > 0
                ? workspaceId
                : null;
    }

    public void Remover()
    {
        cookies.Delete(
            NomeCookie,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
                Path = "/"
            });
    }
}
