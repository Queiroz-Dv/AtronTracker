using Application.EmailCompositor.Compositores;
using Application.Interfaces.Services;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;

namespace Application.Records.Facade
{
    public sealed record RecuperacaoSenhaFacadeRecord(
        IUsuarioRepository UsuarioRepository,
        IUsuarioIdentityRepository IdentityRepository,
        ILoginRepository LoginRepository,
        ICacheService CacheService,
        IEmailService EmailService,
        IAcessoEmailCompositor EmailCompositor,
        IEnderecoFrontendService EnderecoFrontendService,
        ITokenTemporarioService TokenTemporarioService);
}