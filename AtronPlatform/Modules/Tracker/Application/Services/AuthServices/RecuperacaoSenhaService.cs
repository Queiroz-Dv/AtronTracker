using Application.DTO;
using Application.DTO.Request;
using Application.EmailCompositor.Compositores;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.Services.AuthServices
{
    public class RecuperacaoSenhaService(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository identityRepository,
        ILoginRepository loginRepository,
        ICacheService cacheService,
        IEmailService emailService,
        IAcessoEmailCompositor emailCompositor,
        IEnderecoFrontendService enderecoFrontendService,
        ITokenTemporarioService tokenTemporarioService) : IRecuperacaoSenhaService
    {
        private const int ValidadeEmHoras = 24;

        public async Task<Resultado> SolicitarAsync(SolicitarRecuperacaoSenhaRequest request)
        {
            var respostaPublica = Resultado.Sucesso(AuthResource.Mensagem_EnvioDeEmail);

            if (request.Identificador.IsNullOrEmpty())
                return respostaPublica;

            var identificador = request.Identificador.NormalizeIdentifier();

            var usuario = identificador.IdentifierIsEmail()
                ? await usuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
                : await usuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador.NormalizeUserCodeIdentifier());

            if (usuario.IsNullable())
                return respostaPublica;

            if (usuario.Inativo)
                return respostaPublica;

            var temporario = tokenTemporarioService.Criar();

            var dados = new DadosTemporarios
            {
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = await identityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo),
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var cache = new CacheInfo<DadosTemporarios>(new ChaveCache(ECacheKeysInfo.DadosTemporarios, temporario.Hash)) { EntityInfo = dados };

            cacheService.GravarCache(cache, TimeSpan.FromHours(ValidadeEmHoras));
            var uri = enderecoFrontendService.ObterUriBase();
            var link = $"{uri}/trocar-senha#token={temporario.Valor}";

            var parametrosEmailDTO = new ParametrosEmailDTO()
            {
                Link = link,
                Email = usuario.Email,
                UsuarioNome = usuario.Nome,
                Validade = ValidadeEmHoras
            };

            var email = emailCompositor.ComporRecuperacaoSenha(parametrosEmailDTO);

            if (email.TeveFalha)
                return respostaPublica;

            await emailService.EnviarAsync(email.Dados);

            return respostaPublica;
        }

        public async Task<Resultado> TrocarAsync(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdentificadorTemporario))
                return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var hash = tokenTemporarioService.ObterHash(request.IdentificadorTemporario);
            var chave = new ChaveCache(ECacheKeysInfo.DadosTemporarios, hash);
            var dados = cacheService.ObterCache<DadosTemporarios>(chave);

            if (dados == null)
                return Resultado.Falha(AuthResource.Erro_CacheExpiradoNaTrocaDeSenha);

            var senha = request.NovaSenha;
            var repetir = request.RepetirSenha;

            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(repetir))
                return Resultado.Falha(AuthResource.Erro_SenhaInvalida);

            if (senha != repetir)
                return Resultado.Falha(AuthResource.Erro_SenhasDivergentes);

            if (!await identityRepository.RedefinirSenhaAsync(dados.UsuarioCodigo, dados.Token, senha))
                return Resultado.Falha(AuthResource.Erro_AtualizarSenha);

            await loginRepository.AtualizarSenhaUsuario(dados.UsuarioCodigo, senha);
            cacheService.RemoverCache(chave);

            return Resultado.Sucesso(AuthResource.Mensagem_SenhaAlterada);
        }
    }
}