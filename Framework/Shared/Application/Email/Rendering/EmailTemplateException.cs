namespace Shared.Application.Email.Rendering;

public sealed class EmailTemplateException : Exception
{
    public EmailTemplateException(string message)
        : base(message)
    {
    }
}
