using Shared.Application.DTOS.Email;

namespace Shared.Application.Interfaces.Service
{
    public interface IEmailService
    {
        Task EnviarAsync(EmailMessage message);
    }
}