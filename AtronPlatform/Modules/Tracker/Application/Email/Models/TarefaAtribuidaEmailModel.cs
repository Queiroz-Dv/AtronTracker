using Shared.Application.Email.Models;

namespace Application.Email.Models;

public sealed record TarefaAtribuidaEmailModel
{
    public string NomeUsuario { get; init; }

    public string Titulo { get; init; }

    [EmailTemplateOptional]
    public string Conteudo { get; init; }

    public string DataInicial { get; init; }

    public string DataFinal { get; init; }

    public string Estado { get; init; }
}
