namespace AtronStock.Application.Resources;

using System.Globalization;
using System.Resources;

public static class EstoqueNotificacaoResource
{
    private static readonly ResourceManager ResourceManager = new(
        "AtronStock.Application.Resources.EstoqueNotificacaoResource",
        typeof(EstoqueNotificacaoResource).Assembly);

    public static string Titulo_SaidaEstoqueRegistrada =>
        ResourceManager.GetString(nameof(Titulo_SaidaEstoqueRegistrada), CultureInfo.CurrentUICulture)!;

    public static string Mensagem_SaidaEstoqueRegistrada =>
        ResourceManager.GetString(nameof(Mensagem_SaidaEstoqueRegistrada), CultureInfo.CurrentUICulture)!;
}
