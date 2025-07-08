using Shared.DTO.Email;

namespace Shared.Interfaces.Email
{
    public interface IEmailService
    {
        Task EnviarAsync(EmailMessage message);
    }
}