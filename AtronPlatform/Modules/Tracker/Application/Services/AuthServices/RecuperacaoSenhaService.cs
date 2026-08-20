using Application.DTO.Request;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Records.Usuario;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class RecuperacaoSenhaService(RecuperacaoSenhaContextRecord context) : IRecuperacaoSenhaService
    {
        private const int ValidadeEmHoras = 24;

        public async Task<Resultado> SolicitarAsync(SolicitarRecuperacaoSenhaRequest request)
        {
            var respostaPublica = Resultado.Sucesso(AuthResource.Mensagem_EnvioDeEmail);

            if (string.IsNullOrWhiteSpace(request.Identificador))
                return respostaPublica;

            var identificador = request.Identificador.NormalizeIdentifier();

            var usuario = identificador.IdentifierIsEmail()
                ? await context.UsuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
                : await context.UsuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador.NormalizeUserCodeIdentifier());

            if (usuario == null)
                return respostaPublica;

            if (usuario.Inativo)
                return respostaPublica;

            var temporario = context.TokenTemporarioService.Criar();

            var dados = new DadosTemporarios
            {
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = await context.IdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo),
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var cache = new CacheInfo<DadosTemporarios>(new ChaveCache(ECacheKeysInfo.DadosTemporarios, temporario.Hash)) { EntityInfo = dados };

            context.CacheService.GravarCache(cache, TimeSpan.FromHours(ValidadeEmHoras));
            var uri = context.EnderecoFrontendService.ObterUriBase();
            var link = $"{uri}/trocar-senha#token={temporario.Valor}";
            try
            {
                var recuperacao = new RecuperacaoSenhaEmailParametrosRecord(usuario.Email, usuario.Nome, link, ValidadeEmHoras);
                var email = context.EmailCompositor.ComporRecuperacaoSenha(recuperacao);

                if (email.TeveFalha)
                    return respostaPublica;

                await context.EmailService.EnviarAsync(email.Dados);
            }
            catch
            {
            }

            return respostaPublica;
        }

        public async Task<Resultado> TrocarAsync(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdentificadorTemporario))
                return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var hash = context.TokenTemporarioService.ObterHash(request.IdentificadorTemporario);
            var chave = new ChaveCache(ECacheKeysInfo.DadosTemporarios, hash);
            var dados = context.CacheService.ObterCache<DadosTemporarios>(chave);

            if (dados == null)
                return Resultado.Falha(AuthResource.Erro_CacheExpiradoNaTrocaDeSenha);

            var senha = request.NovaSenha;
            var repetir = request.RepetirSenha;

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
