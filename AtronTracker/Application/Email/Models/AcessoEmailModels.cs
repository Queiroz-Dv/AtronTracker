using Shared.Application.Email.Models;

namespace Application.Email.Models;

public sealed record ConfirmacaoCadastroEmailModel
{
    public string Nome { get; init; }
    public string Codigo { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record RecuperacaoSenhaEmailModel
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record ConfirmacaoConcluidaEmailModel
{
    public string Nome { get; init; }
}

public sealed record PrimeiroAcessoEmailModel
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record AlteracaoEmailEmailModel
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
}

public sealed record ReativacaoContaEmailModel
{
    public string Nome { get; init; }
    public string Codigo { get; init; }
}
