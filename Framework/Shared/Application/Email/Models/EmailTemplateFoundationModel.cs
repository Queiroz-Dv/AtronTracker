namespace Shared.Application.Email.Models;

/// <summary>
/// Modelo tipado usado para validar a infraestrutura comum antes da migração dos fluxos reais.
/// </summary>
public sealed record EmailTemplateFoundationModel
{
    public string Nome { get; init; }

    public string Conteudo { get; init; }

    [EmailTemplateUrl]
    public string Link { get; init; }

    public string TextoLink { get; init; }
}
