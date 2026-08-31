using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Infrastructure.Configuration;

public sealed class WorkspaceAtualService(IResponseCookies cookies)
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
}
