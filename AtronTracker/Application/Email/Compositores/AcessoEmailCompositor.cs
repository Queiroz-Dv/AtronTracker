using Application.Email.Models;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using System.Globalization;

namespace Application.Email.Compositores;

public sealed class AcessoEmailCompositor : IAcessoEmailCompositor
{
    private const string PrefixoTemplate = "Application.Email.Templates.pt-BR.";
    private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");
    private readonly IEmailTemplateRenderer _renderer;

    public AcessoEmailCompositor(IEmailTemplateRenderer renderer)
    {
        _renderer = renderer;
    }

    public EmailRequest ComporConfirmacaoCadastro(string destinatario, string nome, string codigo, string link, int validadeHoras)
    {
        return Renderizar(
            "confirmacao-cadastro.html",
            EmailResource.Assunto_ConfirmeCadastro,
            EmailResource.Titulo_ConfirmacaoCadastro,
            new ConfirmacaoCadastroEmailModel
            {
                Nome = nome,
                Codigo = codigo,
                Link = link,
                ValidadeHoras = validadeHoras.ToString(CulturaPtBr)
            },
            destinatario);
    }

    public EmailRequest ComporRecuperacaoSenha(string destinatario, string nome, string link, int validadeHoras)
    {
        return Renderizar(
            "recuperacao-senha.html",
            EmailResource.Assunto_RecuperacaoSenha,
            EmailResource.Titulo_RecuperacaoSenha,
            new RecuperacaoSenhaEmailModel
            {
                Nome = nome,
                Link = link,
                ValidadeHoras = validadeHoras.ToString(CulturaPtBr)
            },
            destinatario);
    }

    public EmailRequest ComporConfirmacaoConcluida(string destinatario, string nome)
    {
        return Renderizar(
            "confirmacao-concluida.html",
            EmailResource.Assunto_EmailConfirmado,
            EmailResource.Titulo_EmailConfirmado,
            new ConfirmacaoConcluidaEmailModel { Nome = nome },
            destinatario);
    }

    public EmailRequest ComporPrimeiroAcesso(string destinatario, string nome, string link, int validadeHoras)
    {
        return Renderizar(
            "primeiro-acesso.html",
            EmailResource.Assunto_PrimeiroAcesso,
            EmailResource.Titulo_PrimeiroAcesso,
            new PrimeiroAcessoEmailModel
            {
                Nome = nome,
                Link = link,
                ValidadeHoras = validadeHoras.ToString(CulturaPtBr)
            },
            destinatario);
    }

    public EmailRequest ComporAlteracaoEmail(string destinatario, string nome, string link)
    {
        return Renderizar(
            "alteracao-email.html",
            EmailResource.Assunto_AlteracaoEmail,
            EmailResource.Titulo_AlteracaoEmail,
            new AlteracaoEmailEmailModel { Nome = nome, Link = link },
            destinatario);
    }

    public EmailRequest ComporReativacaoConta(string destinatario, string nome, string codigo)
    {
        return Renderizar(
            "reativacao-conta.html",
            EmailResource.Assunto_ReativacaoConta,
            EmailResource.Titulo_ReativacaoConta,
            new ReativacaoContaEmailModel { Nome = nome, Codigo = codigo },
            destinatario);
    }

    private EmailRequest Renderizar<TModel>(
        string arquivo,
        string assunto,
        string titulo,
        TModel model,
        string destinatario)
        where TModel : class
    {
        var template = new EmailTemplateDefinition(
            typeof(AcessoEmailCompositor).Assembly,
            $"{PrefixoTemplate}{arquivo}",
            assunto,
            titulo);

        return _renderer.Renderizar(template, model, [destinatario]);
    }
}
