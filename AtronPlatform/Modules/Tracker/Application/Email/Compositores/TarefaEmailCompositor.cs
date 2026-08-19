using Application.DTO;
using Application.Extensions;
using Application.Records.Tarefa;
using Application.Resources;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Domain.ValueObjects;

namespace Application.Email.Compositores;

public sealed class TarefaEmailCompositor(IEmailTemplateRenderer renderer) : ITarefaEmailCompositor
{
    private const string TemplateResourceName = "Application.Email.Templates.pt-BR.tarefa-atribuida.html";
    private readonly IEmailTemplateRenderer _renderer = renderer;

    public Resultado<EmailRequest> ComporAtribuicao(TarefaDTO tarefa, UsuarioDTO usuario)
    {
        var assunto = string.Format(TarefaResource.Assunto_EmailTarefaAtribuida, tarefa.Titulo);

        var template = new EmailTemplateDefinition(typeof(TarefaEmailCompositor).Assembly,
            TemplateResourceName, assunto,
            TarefaResource.Titulo_EmailTarefaAtribuida);

        var model = new TarefaAtribuidaEmailModelRecord
        {
            NomeUsuario = $"{usuario.Nome} {usuario.Sobrenome}".Trim(),
            Titulo = tarefa.Titulo,
            Conteudo = tarefa.Conteudo ?? string.Empty,
            DataInicial = tarefa.DataInicial.ToString("dd/MM/yyyy"),
            DataFinal = tarefa.DataFinal.ToString("dd/MM/yyyy"),
            Estado = tarefa.ObterDescricaoEstado()
        };

        return _renderer.Renderizar(template, model, [usuario.Email]);
    }
}
