using Shared.Application.DTOS.Requests;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IEmailService
    {
        Task<Resultado> EnviarAsync(EmailRequest message);
    }
}