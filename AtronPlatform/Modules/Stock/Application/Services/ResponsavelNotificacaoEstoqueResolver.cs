using AtronStock.Application.Interfaces;

namespace AtronStock.Application.Services;

public sealed class ResponsavelNotificacaoEstoqueResolver(string codigoResponsavel)
    : IResponsavelNotificacaoEstoqueResolver
{
    public string ObterCodigoResponsavel()
        => codigoResponsavel ?? string.Empty;
}
