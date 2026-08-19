namespace AtronStock.Application.Providers.Notificacoes;

public sealed class ResponsavelNotificacaoEstoqueProvider(string codigoResponsavel)
{
    public string ObterCodigoResponsavel()
        => codigoResponsavel ?? string.Empty;
}
