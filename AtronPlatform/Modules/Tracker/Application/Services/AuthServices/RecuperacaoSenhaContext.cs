using Application.Email.Compositores;
using Application.Interfaces.Services;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;

namespace Application.Services.AuthServices
{
    public sealed record RecuperacaoSenhaContext(
        IUsuarioRepository UsuarioRepository,
        IUsuarioIdentityRepository IdentityRepository,
        ILoginRepository LoginRepository,
        ICacheService CacheService,
        IEmailService EmailService,
        IAcessoEmailCompositor EmailCompositor,
        IEnderecoFrontendService EnderecoFrontendService,
        ITokenTemporarioService TokenTemporarioService);
}
