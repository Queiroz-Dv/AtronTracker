using Application.DTO.Request;
using Application.Email.Models;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Services.AuthServices.Bases;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Application.Services.AuthServices
{
    public class RecuperacaoSenhaService(RecuperacaoSenhaContext context)
        : AuthUriBaseService(context.HttpContextAccessor), IRecuperacaoSenhaService
    {
        private const int ValidadeEmHoras = 24;

        public async Task<Resultado> SolicitarAsync(SolicitarRecuperacaoSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identificador))
                return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var identificador = request.Identificador.NormalizeIdentifier();

            var usuario = identificador.IdentifierIsEmail()
                ? await context.UsuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
                : await context.UsuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador.NormalizeUserCodeIdentifier());

            if (usuario == null)
                return Resultado.Falha(AuthResource.Erro_UsuarioNaoEncontrado);

            if (usuario.Inativo)
                return Resultado.Falha(AuthResource.Erro_UsuarioInativo);

            var temporario = CryptoHelper.GerarIdentificadorTemporario(usuario.Codigo);

            var dados = new DadosTemporarios
            {
                IdentificadorTemporario = temporario,
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = await context.IdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo),
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var cache = new CacheInfo<DadosTemporarios>(new ChaveCache(ECacheKeysInfo.DadosTemporarios, temporario)) { EntityInfo = dados };

            context.CacheService.GravarCache(cache, TimeSpan.FromHours(ValidadeEmHoras));
            var uri = ObterUri(request.ClientUri);
            var encoder = HttpUtility.UrlEncode(CryptoHelper.EncryptCryptoJsAes(temporario));

            var link = $"{uri}/trocar-senha?id={encoder}";
            try
            {
                var recuperacao = new RecuperacaoSenhaEmailParametros(usuario.Email, usuario.Nome, link, ValidadeEmHoras);
                var email = context.EmailCompositor.ComporRecuperacaoSenha(recuperacao);

                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                var envio = await context.EmailService.EnviarAsync(email.Dados);

                return envio.TeveFalha ? Resultado.Falha(envio.Messages) : Resultado.Sucesso();
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }
        }

        public async Task<Resultado> TrocarAsync(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdentificadorTemporario))
                return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var chave = new ChaveCache(ECacheKeysInfo.DadosTemporarios, request.IdentificadorTemporario);
            var dados = context.CacheService.ObterCache<DadosTemporarios>(chave);

            if (dados == null)
                return Resultado.Falha(AuthResource.Erro_CacheExpiradoNaTrocaDeSenha);

            var senha = CryptoHelper.DecryptCryptoJsAes(request.NovaSenha);
            var repetir = CryptoHelper.DecryptCryptoJsAes(request.RepetirSenha);

            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(repetir))
                return Resultado.Falha(AuthResource.Erro_SenhaInvalida);

            if (senha != repetir)
                return Resultado.Falha(AuthResource.Erro_SenhasDivergentes);

            if (!await context.IdentityRepository.RedefinirSenhaAsync(dados.UsuarioCodigo, dados.Token, senha))
                return Resultado.Falha(AuthResource.Erro_AtualizarSenha);

            await context.LoginRepository.AtualizarSenhaUsuario(dados.UsuarioCodigo, senha);
            context.CacheService.RemoverCache(chave);

            return Resultado.Sucesso(AuthResource.Mensagem_SenhaAlterada);
        }
    }
}