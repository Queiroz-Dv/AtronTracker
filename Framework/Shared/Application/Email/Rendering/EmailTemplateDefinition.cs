using System.Reflection;

namespace Shared.Application.Email.Rendering;

/// <summary>
/// Identifica um template incorporado e os metadados necessários para gerar o e-mail.
/// </summary>
public sealed record EmailTemplateDefinition(
    Assembly TemplateAssembly,
    string TemplateResourceName,
    string Assunto,
    string Titulo);
