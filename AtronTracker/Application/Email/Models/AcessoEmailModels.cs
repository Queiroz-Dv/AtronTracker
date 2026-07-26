using Shared.Application.Email.Models;

namespace Application.Email.Models;

/// <summary>
/// Dados necessarios para compor o e-mail de confirmacao de cadastro.
/// </summary>
public sealed record ConfirmacaoCadastroEmailParametros(
    string Destinatario,
    string Nome,
    string Codigo,
    string Link,
    int ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de recuperacao de senha.
/// </summary>
public sealed record RecuperacaoSenhaEmailParametros(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de primeiro acesso.
/// </summary>
public sealed record PrimeiroAcessoEmailParametros(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

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

public sealed record AlteracaoEmailModel
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
