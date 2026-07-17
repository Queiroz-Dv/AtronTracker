using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Models;
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

    public EmailRequest Renderizar<TModel>(
        EmailTemplateDefinition template,
        TModel model,
        IEnumerable<string> destinatarios)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(destinatarios);

        if (string.IsNullOrWhiteSpace(template.Assunto))
            throw new EmailTemplateException("O assunto do e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(template.Titulo))
            throw new EmailTemplateException("O título do template é obrigatório.");

        var emailsDestino = destinatarios
            .Where(destinatario => !string.IsNullOrWhiteSpace(destinatario))
            .Select(destinatario => destinatario.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (emailsDestino.Count == 0)
            throw new EmailTemplateException("Ao menos um destinatário é obrigatório.");

        var templateConteudo = CarregarTemplate(template.TemplateAssembly, template.TemplateResourceName);
        var conteudoRenderizado = RenderizarCampos(templateConteudo, model);
        var templateBase = CarregarTemplate(typeof(EmailTemplateRenderer).Assembly, EmailTemplateResourceNames.Base);
        var html = RenderizarBase(templateBase, template.Titulo, conteudoRenderizado);

        return new EmailRequest
        {
            Assunto = template.Assunto,
            Mensagem = html,
            EmailsDestino = emailsDestino
        };
    }

    private static string CarregarTemplate(Assembly assembly, string resourceName)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        if (string.IsNullOrWhiteSpace(resourceName) ||
            !resourceName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            resourceName.Contains("..", StringComparison.Ordinal) ||
            resourceName.Contains('/') ||
            resourceName.Contains('\\'))
        {
            throw new EmailTemplateException("O nome do template incorporado é inválido.");
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            throw new EmailTemplateException($"Template incorporado não encontrado: {resourceName}.");

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    private static string RenderizarCampos<TModel>(string template, TModel model)
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
                throw new EmailTemplateException($"O modelo {typeof(TModel).Name} não fornece o campo obrigatório {token}.");

            var rawValue = property.GetValue(model);
            var value = ConverterValor(rawValue);
            if (string.IsNullOrWhiteSpace(value) &&
                !property.IsDefined(typeof(EmailTemplateOptionalAttribute), inherit: true))
                throw new EmailTemplateException($"O campo obrigatório {token} não foi informado.");

            if (property.IsDefined(typeof(EmailTemplateUrlAttribute), inherit: true))
                value = ValidarUrl(value, token);

            resultado = resultado.Replace(
                $"{{{{{token}}}}}",
                WebUtility.HtmlEncode(value),
                StringComparison.Ordinal);
        }

        if (TokenPattern.IsMatch(resultado))
            throw new EmailTemplateException("O template contém campos obrigatórios não renderizados.");

        return resultado;
    }

    private static string RenderizarBase(string templateBase, string titulo, string conteudoRenderizado)
    {
        var html = templateBase
            .Replace("{{Titulo}}", WebUtility.HtmlEncode(titulo), StringComparison.Ordinal)
            .Replace("{{Ano}}", DateTime.UtcNow.Year.ToString(CulturaPtBr), StringComparison.Ordinal)
            .Replace("{{Conteudo}}", conteudoRenderizado, StringComparison.Ordinal);

        if (TokenPattern.IsMatch(html))
            throw new EmailTemplateException("O template base contém campos obrigatórios não renderizados.");

        return html;
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

    private static string ValidarUrl(string value, string token)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new EmailTemplateException($"O campo {token} deve conter uma URL HTTP ou HTTPS absoluta.");
        }

        return uri.AbsoluteUri;
    }
}