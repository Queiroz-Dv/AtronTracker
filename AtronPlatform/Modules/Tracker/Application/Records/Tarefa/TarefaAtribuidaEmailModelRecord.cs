using Shared.Application.Email.Models;

namespace Application.Records.Tarefa;

public sealed record TarefaAtribuidaEmailModelRecord
{
    public string NomeUsuario { get; init; }

    public string Titulo { get; init; }

    [EmailTemplateOptional]
    public string Conteudo { get; init; }

    public string DataInicial { get; init; }

    public string DataFinal { get; init; }

    public string Estado { get; init; }
}
