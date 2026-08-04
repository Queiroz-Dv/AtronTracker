using Application.Email.Compositores;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;

namespace Application.Services.AuthServices
{
    public sealed record CadastroUsuarioContext(
        IUsuarioRepository UsuarioRepository,
        IUsuarioIdentityRepository IdentityRepository,
        IEmailService EmailService,
        IAcessoEmailCompositor EmailCompositor,
        IValidador<Application.DTO.Request.UsuarioRegistroRequest> Validador,
        IEnderecoFrontendService EnderecoFrontendService,
        IConfirmacaoEmailRepository ConfirmacaoRepository,
        IConfirmacaoEmailCodigoService ConfirmacaoCodigoService);
}
