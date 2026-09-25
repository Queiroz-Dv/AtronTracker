namespace Shared.Extensions;

public sealed class EmailTemplateException : Exception
{
    public EmailTemplateException(string message) : base(message) { }
}
