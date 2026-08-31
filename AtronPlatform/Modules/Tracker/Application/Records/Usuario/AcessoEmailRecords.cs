using Shared.Application.Email.Models;

namespace Application.Records.Usuario;

/// <summary>
/// Dados necessarios para compor o e-mail de confirmacao de cadastro.
/// </summary>
public abstract record EmailParametrosRecord(
    string Destinatario,
    string Nome);

public abstract record EmailComLinkParametrosRecord(
    string Destinatario,
    string Nome,
    string Link)
    : EmailParametrosRecord(Destinatario, Nome);

public abstract record EmailComLinkExpiravelParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras)
    : EmailComLinkParametrosRecord(Destinatario, Nome, Link);

public sealed record ConfirmacaoCadastroEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Codigo,
    string Link,
    int ValidadeHoras)
    : EmailComLinkExpiravelParametrosRecord(
        Destinatario,
        Nome,
        Link,
        ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de recuperacao de senha.
/// </summary>
public sealed record RecuperacaoSenhaEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras)
    : EmailComLinkExpiravelParametrosRecord(
        Destinatario,
        Nome,
        Link,
        ValidadeHoras);

/// <summary>
/// Dados necessarios para compor o e-mail de primeiro acesso.
/// </summary>
public sealed record PrimeiroAcessoEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras)
    : EmailComLinkExpiravelParametrosRecord(
        Destinatario,
        Nome,
        Link,
        ValidadeHoras);

public sealed record ConfirmacaoCadastroCriadaRecord(
    string Link,
    string Identificador,
    int ValidadeHoras);

public abstract record EmailNomeModelRecord
{
    public string Nome { get; init; } = string.Empty;
}

public abstract record EmailComLinkModelRecord : EmailNomeModelRecord
{
    [EmailTemplateUrl]
    public string Link { get; init; } = string.Empty;
}

public abstract record EmailComLinkExpiravelModelRecord : EmailComLinkModelRecord
{
    public string ValidadeHoras { get; init; } = string.Empty;
}

public sealed record ConfirmacaoCadastroEmailModelRecord
    : EmailComLinkExpiravelModelRecord
{
    public string Codigo { get; init; } = string.Empty;
}

public sealed record RecuperacaoSenhaEmailModelRecord
    : EmailComLinkExpiravelModelRecord;

public sealed record ConfirmacaoConcluidaEmailModelRecord
    : EmailNomeModelRecord;

public sealed record PrimeiroAcessoEmailModelRecord
    : EmailComLinkExpiravelModelRecord;

public sealed record AlteracaoEmailModelRecord
    : EmailComLinkModelRecord;

public sealed record ReativacaoContaEmailModelRecord : EmailNomeModelRecord
{
    public string Codigo { get; init; } = string.Empty;
}
