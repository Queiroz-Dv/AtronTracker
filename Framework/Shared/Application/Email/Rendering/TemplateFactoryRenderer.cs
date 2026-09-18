using Shared.Application.Email.Models;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Net;
using System.Reflection;
using System.Text;
using Shared.Extensions;
using System.Text.RegularExpressions;

namespace Shared.Application.Email.Rendering;

public class TemplateFactoryRenderer
{
    public static Resultado<string> Executar(Assembly assembly, string resourceName)
    {
        if (assembly.IsNullable() ||
            resourceName.IsNullOrEmpty() ||
            !resourceName.EndsWith(".html") ||
            resourceName.Contains("..") ||
            resourceName.Contains('/') ||
            resourceName.Contains('\\'))
        {
            return Resultado<string>.Falha(EmailResource.Erro_TemplateNomeInvalido);
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream.IsNullable())
            return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateNaoEncontrado, resourceName));

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return Resultado<string>.Sucesso(reader.ReadToEnd());
    }
    
    public static Resultado<string> ObterCamposRenderizados<TModel>(string template, TModel model, Regex tokenPattern)
    {        
         var properties = typeof(TModel)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead)
            .ToDictionary(property => property.Name);

        var tokens = tokenPattern.Matches(template)
            .Select(match => match.Groups["nome"].Value)
            .Distinct()
            .ToList();

        var resultado = template;
        foreach (var token in tokens)
        {
            if (!properties.TryGetValue(token, out var property))
                return Resultado<string>.Falha(string.Format(EmailResource.Erro_TemplateModeloSemCampo, typeof(TModel).Name, token));

            var rawValue = property.GetValue(model);
            var value = ConverterValor(rawValue);

            if (value.IsNullOrEmpty() &&
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

        if (tokenPattern.IsMatch(resultado))
            return Resultado<string>.Falha(EmailResource.Erro_TemplateCamposNaoRenderizados);

        return Resultado<string>.Sucesso(resultado);
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

    private static string ConverterValor(object value) => value switch
    {
        null => string.Empty,
        IFormattable formattable => formattable.ToString(),
        _ => value.ToString() ?? string.Empty
    };

    public static Resultado<string> ObterBaseRenderizada(string templateBase, string titulo, string conteudoRenderizado, Regex tokenPattern)
    {
        var html = templateBase
            .Replace("{{Titulo}}", WebUtility.HtmlEncode(titulo), StringComparison.Ordinal)
            .Replace("{{Ano}}", DateTime.UtcNow.Year.ToString("pt-BR"), StringComparison.Ordinal)
            .Replace("{{Conteudo}}", conteudoRenderizado, StringComparison.Ordinal);

        if (tokenPattern.IsMatch(html))
            return Resultado<string>.Falha(EmailResource.Erro_TemplateBaseCamposNaoRenderizados);

        return Resultado<string>.Sucesso(html);
    }
}
