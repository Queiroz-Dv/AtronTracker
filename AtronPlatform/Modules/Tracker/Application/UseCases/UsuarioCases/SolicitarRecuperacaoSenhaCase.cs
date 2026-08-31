using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Records.Usuario;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class SolicitarRecuperacaoSenhaCase(
    IUsuarioRepository usuarioRepository,
    IUsuarioIdentityRepository identityRepository,
    ICacheService cacheService,
    IEmailService emailService,
    IAcessoEmailCompositor emailCompositor,
    IEnderecoFrontendService enderecoFrontendService,
    ITokenTemporarioService tokenTemporarioService)
{
    private const int ValidadeEmHoras = 24;

    public async Task<Resultado> ExecutarAsync(SolicitarRecuperacaoSenhaRequest request)
    {
        var respostaPublica = Resultado.Sucesso(AuthResource.Mensagem_EnvioDeEmail);

        if (request.Identificador.IsNullOrEmpty())
            return respostaPublica;

        var identificador = request.Identificador;

        var usuario = identificador.IdentifierIsEmail()
            ? await usuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
            : await usuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador);

        if (usuario == null || usuario.Inativo)
            return respostaPublica;

        var temporario = tokenTemporarioService.Criar();

        var dados = new DadosTemporarios
        {
            UsuarioCodigo = usuario.Codigo,
            Email = usuario.Email,
            Token = await identityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo),
            DataAlteracaoSenha = DateTime.UtcNow
        };

        var cache = new CacheInfo<DadosTemporarios>(new ChaveCache(ECacheKeysInfo.DadosTemporarios, temporario.Hash))
        {
            EntityInfo = dados
        };

        cacheService.GravarCache(cache, TimeSpan.FromHours(ValidadeEmHoras));
        var uri = enderecoFrontendService.ObterUriBase();
        var link = $"{uri}/trocar-senha#token={temporario.Valor}";

        var recuperacao = new RecuperacaoSenhaEmailParametrosRecord(usuario.Email, usuario.Nome, link, ValidadeEmHoras);
        var email = emailCompositor.ComporRecuperacaoSenha(recuperacao);

        if (email.TeveFalha)
            return respostaPublica;

        await emailService.EnviarAsync(email.Dados);

        return respostaPublica;
    }
}
