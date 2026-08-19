using Application.Records.Usuario;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
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

    public Resultado<EmailRequest> ComporConfirmacaoCadastro(ConfirmacaoCadastroEmailParametrosRecord parametros)
    {
        return Renderizar(
            CriarTemplate(
                "confirmacao-cadastro.html",
                EmailResource.Assunto_ConfirmeCadastro,
                EmailResource.Titulo_ConfirmacaoCadastro),
            new ConfirmacaoCadastroEmailModelRecord
            {
                Nome = parametros.Nome,
                Codigo = parametros.Codigo,
                Link = parametros.Link,
                ValidadeHoras = parametros.ValidadeHoras.ToString(CulturaPtBr)
            },
            parametros.Destinatario);
    }

    public Resultado<EmailRequest> ComporRecuperacaoSenha(RecuperacaoSenhaEmailParametrosRecord parametros)
    {
        return Renderizar(
            CriarTemplate(
                "recuperacao-senha.html",
                EmailResource.Assunto_RecuperacaoSenha,
                EmailResource.Titulo_RecuperacaoSenha),
            new RecuperacaoSenhaEmailModelRecord
            {
                Nome = parametros.Nome,
                Link = parametros.Link,
                ValidadeHoras = parametros.ValidadeHoras.ToString(CulturaPtBr)
            },
            parametros.Destinatario);
    }

    public Resultado<EmailRequest> ComporConfirmacaoConcluida(string destinatario, string nome)
    {
        return Renderizar(
            CriarTemplate(
                "confirmacao-concluida.html",
                EmailResource.Assunto_EmailConfirmado,
                EmailResource.Titulo_EmailConfirmado),
            new ConfirmacaoConcluidaEmailModelRecord { Nome = nome },
            destinatario);
    }

    public Resultado<EmailRequest> ComporPrimeiroAcesso(PrimeiroAcessoEmailParametrosRecord parametros)
    {
        return Renderizar(
            CriarTemplate(
                "primeiro-acesso.html",
                EmailResource.Assunto_PrimeiroAcesso,
                EmailResource.Titulo_PrimeiroAcesso),
            new PrimeiroAcessoEmailModelRecord
            {
                Nome = parametros.Nome,
                Link = parametros.Link,
                ValidadeHoras = parametros.ValidadeHoras.ToString(CulturaPtBr)
            },
            parametros.Destinatario);
    }

    public Resultado<EmailRequest> ComporAlteracaoEmail(string destinatario, string nome, string link)
    {
        return Renderizar(
            CriarTemplate(
                "alteracao-email.html",
                EmailResource.Assunto_AlteracaoEmail,
                EmailResource.Titulo_AlteracaoEmail),
            new AlteracaoEmailModelRecord { Nome = nome, Link = link },
            destinatario);
    }

    public Resultado<EmailRequest> ComporReativacaoConta(string destinatario, string nome, string codigo)
    {
        return Renderizar(
            CriarTemplate(
                "reativacao-conta.html",
                EmailResource.Assunto_ReativacaoConta,
                EmailResource.Titulo_ReativacaoConta),
            new ReativacaoContaEmailModelRecord { Nome = nome, Codigo = codigo },
            destinatario);
    }

    private static EmailTemplateDefinition CriarTemplate(string arquivo, string assunto, string titulo)
    {
        return new EmailTemplateDefinition(
            typeof(AcessoEmailCompositor).Assembly,
            $"{PrefixoTemplate}{arquivo}",
            assunto,
            titulo);
    }

    private Resultado<EmailRequest> Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        string destinatario)
        where TModel : class
    {
        return _renderer.Renderizar(template, model, [destinatario]);
    }
}
