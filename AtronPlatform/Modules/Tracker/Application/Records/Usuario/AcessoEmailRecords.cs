using Shared.Application.Email.Models;

namespace Application.Records.Usuario;

/// <summary>
/// Dados necessarios para compor o e-mail de confirmacao de cadastro.
/// </summary>
public sealed record ConfirmacaoCadastroEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Codigo,
    string Link,
    int ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de recuperacao de senha.
/// </summary>
public sealed record RecuperacaoSenhaEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de primeiro acesso.
/// </summary>
public sealed record PrimeiroAcessoEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

public sealed record ConfirmacaoCadastroEmailModelRecord
{
    public string Nome { get; init; }
    public string Codigo { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record RecuperacaoSenhaEmailModelRecord
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record ConfirmacaoConcluidaEmailModelRecord
{
    public string Nome { get; init; }
}

public sealed record PrimeiroAcessoEmailModelRecord
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
    public string ValidadeHoras { get; init; }
}

public sealed record AlteracaoEmailModelRecord
{
    public string Nome { get; init; }
    [EmailTemplateUrl]
    public string Link { get; init; }
}

public sealed record ReativacaoContaEmailModelRecord
{
    public string Nome { get; init; }
    public string Codigo { get; init; }
}
