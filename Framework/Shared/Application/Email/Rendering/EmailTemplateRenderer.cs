using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Models;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Application.Email.Rendering;

public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private static readonly Regex TokenPattern = new(
        @"\{\{(?<nome>[A-Za-z][A-Za-z0-9]*)\}\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");

    public Resultado<EmailRequest> Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        IEnumerable<string> destinatarios)
        where TModel : class
    {
        try
        {
            if (template is null)
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateDefinicaoObrigatoria);

            if (model is null)
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateModeloObrigatorio);

            if (destinatarios is null)
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateDestinatarioObrigatorio);

            if (string.IsNullOrWhiteSpace(template.Assunto))
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateAssuntoObrigatorio);

            if (string.IsNullOrWhiteSpace(template.Titulo))
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateTituloObrigatorio);

            var emailsDestino = destinatarios
                .Where(destinatario => !string.IsNullOrWhiteSpace(destinatario))
                .Select(destinatario => destinatario.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emailsDestino.Count == 0)
                return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateDestinatarioObrigatorio);

            var templateConteudo = CarregarTemplate(template.TemplateAssembly, template.TemplateResourceName);
            if (templateConteudo.TeveFalha)
                return Resultado<EmailRequest>.Falhas(templateConteudo.Messages);

            var conteudoRenderizado = RenderizarCampos(templateConteudo.Dados, model);
            if (conteudoRenderizado.TeveFalha)
                return Resultado<EmailRequest>.Falhas(conteudoRenderizado.Messages);

            var templateBase = CarregarTemplate(typeof(EmailTemplateRenderer).Assembly, EmailTemplateResourceNames.Base);
            if (templateBase.TeveFalha)
                return Resultado<EmailRequest>.Falhas(templateBase.Messages);

            var html = RenderizarBase(templateBase.Dados, template.Titulo, conteudoRenderizado.Dados);
            if (html.TeveFalha)
                return Resultado<EmailRequest>.Falhas(html.Messages);

            return Resultado<EmailRequest>.Sucesso(new EmailRequest
            {
                Assunto = template.Assunto,
                Mensagem = html.Dados,
                EmailsDestino = emailsDestino
            });
        }
        catch
        {
            return Resultado<EmailRequest>.Falha(EmailResource.Erro_TemplateRenderizacao);
        }
    }

    private static Resultado<string> CarregarTemplate(Assembly assembly, string resourceName)
    {
        if (assembly is null ||
            string.IsNullOrWhiteSpace(resourceName) ||
            !resourceName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            resourceName.Contains("..", StringComparison.Ordinal) ||
            resourceName.Contains('/') ||
            resourceName.Contains('\\'))
        {
            return Resultado<string>.Falha(EmailResource.Erro_TemplateNomeInvalido);
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateNaoEncontrado, resourceName));

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return Resultado<string>.Sucesso(reader.ReadToEnd());
    }

    private static Resultado<string> RenderizarCampos<TModel>(string template, TModel model)
        where TModel : class
    {
        var properties = typeof(TModel)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead)
            .ToDictionary(property => property.Name, StringComparer.Ordinal);

        var tokens = TokenPattern.Matches(template)
            .Select(match => match.Groups["nome"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var resultado = template;
        foreach (var token in tokens)
        {
            if (!properties.TryGetValue(token, out var property))
                return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateModeloSemCampo, typeof(TModel).Name, token));

            var rawValue = property.GetValue(model);
            var value = ConverterValor(rawValue);
            if (string.IsNullOrWhiteSpace(value) &&
                !property.IsDefined(typeof(EmailTemplateOptionalAttribute), inherit: true))
                return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateCampoObrigatorio, token));

            if (property.IsDefined(typeof(EmailTemplateUrlAttribute), inherit: true))
            {
                var url = ValidarUrl(value, token);
                if (url.TeveFalha)
                    return Resultado<string>.Falhas(url.Messages);

                value = url.Dados;
            }

            resultado = resultado.Replace(
                $"{{{{{token}}}}}",
                WebUtility.HtmlEncode(value),
                StringComparison.Ordinal);
        }

        if (TokenPattern.IsMatch(resultado))
            return Resultado<string>.Falha(EmailResource.Erro_TemplateCamposNaoRenderizados);

        return Resultado<string>.Sucesso(resultado);
    }

    private static Resultado<string> RenderizarBase(string templateBase, string titulo, string conteudoRenderizado)
    {
        var html = templateBase
            .Replace("{{Titulo}}", WebUtility.HtmlEncode(titulo), StringComparison.Ordinal)
            .Replace("{{Ano}}", DateTime.UtcNow.Year.ToString(CulturaPtBr), StringComparison.Ordinal)
            .Replace("{{Conteudo}}", conteudoRenderizado, StringComparison.Ordinal);

        if (TokenPattern.IsMatch(html))
            return Resultado<string>.Falha(EmailResource.Erro_TemplateBaseCamposNaoRenderizados);

        return Resultado<string>.Sucesso(html);
    }

    private static string ConverterValor(object value)
    {
        return value switch
        {
            null => string.Empty,
            IFormattable formattable => formattable.ToString(null, CulturaPtBr),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static Resultado<string> ValidarUrl(string value, string token)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateUrlInvalida, token));
        }

        return Resultado<string>.Sucesso(uri.AbsoluteUri);
    }
}