using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Interfaces.Services;
using Application.Records.Usuario;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Application.Services;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class EnviarEmailPrimeiroAcessoCase(
        IUsuarioIdentityRepository usuarioIdentityRepository,
        ITokenTemporarioService tokenTemporarioService,
        ICacheService cacheService,
        IEnderecoFrontendService enderecoFrontendService,
        IAcessoEmailCompositor emailCompositor,
        IEmailService emailService)
    {
        private readonly ICacheService _cacheService = cacheService;
        private readonly ITokenTemporarioService _tokenTemporarioService = tokenTemporarioService;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly IAcessoEmailCompositor _emailCompositor = emailCompositor;
        private readonly IEmailService _emailService = emailService;

        private const int ValidadeConvitePrimeiroAcessoEmHoras = 24;
        private readonly IEnderecoFrontendService _enderecoFrontendService = enderecoFrontendService;

        public async Task<Resultado> ExecutarAsync(Usuario usuario)
        {
            var token = await _usuarioIdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo);
            if (string.IsNullOrWhiteSpace(token))
                return Resultado.Falha(AuthResource.Erro_GerarLinkPrimeiroAcesso);

            var identificadorTemporario = _tokenTemporarioService.Criar();
            var dadosTemporarios = new DadosTemporarios
            {
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = token,
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var chaveCache = new ChaveCache(ECacheKeysInfo.DadosTemporarios, identificadorTemporario.Hash);
            var cacheInfo = new CacheInfo<DadosTemporarios>(chaveCache);
            cacheInfo.VincularDadosTemporarios(dadosTemporarios);

            _cacheService.GravarCache(cacheInfo, TimeSpan.FromHours(ValidadeConvitePrimeiroAcessoEmHoras));

            var uriBase = _enderecoFrontendService.ObterUriBase();
            var link = $"{uriBase}/trocar-senha#token={identificadorTemporario.Valor}";

            Resultado resultadoEmail;
            try
            {
                var email = _emailCompositor.ComporPrimeiroAcesso(new PrimeiroAcessoEmailParametrosRecord(
                    usuario.Email,
                    usuario.Nome,
                    link,
                    ValidadeConvitePrimeiroAcessoEmHoras));

                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                resultadoEmail = await _emailService.EnviarAsync(email.Dados);
            }
            catch
            {
                _cacheService.RemoverCache(chaveCache);
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }

            if (resultadoEmail.TeveFalha)
            {
                _cacheService.RemoverCache(chaveCache);
                return Resultado.Falha(resultadoEmail.Messages);
            }

            return Resultado.Sucesso();
        }
    }
}