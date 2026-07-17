using Application.DTO;
using Application.Email.Models;
using Application.Resources;
using Domain.Entities;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Extensions;
using System.Globalization;

namespace Application.Email.Compositores;

public sealed class TarefaEmailCompositor : ITarefaEmailCompositor
{
    private const string TemplateResourceName = "Application.Email.Templates.pt-BR.tarefa-atribuida.html";
    private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");
    private readonly IEmailTemplateRenderer _renderer;

    public TarefaEmailCompositor(IEmailTemplateRenderer renderer)
    {
        _renderer = renderer;
    }

    public EmailRequest ComporAtribuicao(TarefaDTO tarefa, Usuario usuario)
    {
        var assunto = string.Format(CulturaPtBr, TarefaResource.Assunto_EmailTarefaAtribuida, tarefa.Titulo);
        var template = new EmailTemplateDefinition(
            typeof(TarefaEmailCompositor).Assembly,
            TemplateResourceName,
            assunto,
            TarefaResource.Titulo_EmailTarefaAtribuida);

        var model = new TarefaAtribuidaEmailModel
        {
            NomeUsuario = $"{usuario.Nome} {usuario.Sobrenome}".Trim(),
            Titulo = tarefa.Titulo,
            Conteudo = tarefa.Conteudo ?? string.Empty,
            DataInicial = tarefa.DataInicial.ToString("dd/MM/yyyy", CulturaPtBr),
            DataFinal = tarefa.DataFinal.ToString("dd/MM/yyyy", CulturaPtBr),
            Estado = ObterDescricaoEstado(tarefa)
        };

        return _renderer.Renderizar(template, model, [usuario.Email]);
    }

    private static string ObterDescricaoEstado(TarefaDTO tarefa)
    {
        return tarefa.EstadoDaTarefa is not null && !tarefa.EstadoDaTarefa.Descricao.IsNullOrEmpty()
            ? tarefa.EstadoDaTarefa.Descricao
            : TarefaResource.Descricao_EstadoNaoInformado;
    }
}
