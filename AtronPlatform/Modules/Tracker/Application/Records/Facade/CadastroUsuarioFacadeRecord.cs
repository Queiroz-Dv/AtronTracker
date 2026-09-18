using Application.DTO.Request;
using Application.EmailCompositor.Compositores;
using Application.Interfaces.Services;
using Application.UseCases.EmailCases;
using Application.UseCases.UsuarioCases;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;

namespace Application.Records.Facade
{
    public sealed record CadastroUsuarioFacadeRecord(
        VerificarUsuarioExistenteCase VerificarUsuarioExistenteCase,
        ProcessarEnvioEmailConfirmacaoCase ProcessarEnvioEmailConfirmacaoCase,
        IUsuarioRepository UsuarioRepository,
        IUsuarioIdentityRepository IdentityRepository,
        IEmailService EmailService,
        IAcessoEmailCompositor EmailCompositor,
        IValidador<UsuarioRegistroRequest> Validador,
        IEnderecoFrontendService EnderecoFrontendService,
        IConfirmacaoEmailRepository ConfirmacaoRepository,
        IConfirmacaoEmailCodigoService ConfirmacaoCodigoService);
}