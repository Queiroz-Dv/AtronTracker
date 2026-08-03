namespace Shared.Application.Email.Models;

/// <summary>
/// Indica que o valor de uma propriedade deve ser uma URL HTTP ou HTTPS absoluta.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EmailTemplateUrlAttribute : Attribute
{
}
