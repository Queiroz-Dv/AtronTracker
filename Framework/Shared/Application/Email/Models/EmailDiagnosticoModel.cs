namespace Shared.Application.Email.Models;

public sealed record EmailDiagnosticoModel
{
    public string Mensagem { get; init; }

    public string Provedor { get; init; }

    public string Host { get; init; }

    public string Remetente { get; init; }

    public string DataHora { get; init; }
}
