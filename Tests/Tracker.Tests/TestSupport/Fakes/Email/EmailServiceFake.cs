using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Tracker.Tests.TestSupport.Fakes.Email;

internal sealed class EmailServiceFake(Resultado? resultado = null) : IEmailService
{
    public EmailRequest? UltimoRequest { get; private set; }
    public int QuantidadeEnvios { get; private set; }

    public Task<Resultado> EnviarAsync(EmailRequest message)
    {
        QuantidadeEnvios++;
        UltimoRequest = message;
        return Task.FromResult(resultado ?? Resultado.Sucesso());
    }
}
