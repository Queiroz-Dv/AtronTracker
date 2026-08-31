using Application.DTO.Request;
using Application.Interfaces.Services;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class TrocarSenhaCase(
    IUsuarioIdentityRepository identityRepository,
    ILoginRepository loginRepository,
    ICacheService cacheService,
    ITokenTemporarioService tokenTemporarioService)
{
    public async Task<Resultado> ExecutarAsync(RedefinirSenhaRequest request)
    {
        if (request.IdentificadorTemporario.IsNullOrEmpty())
            return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

        var hash = tokenTemporarioService.ObterHash(request.IdentificadorTemporario);
        var chave = new ChaveCache(ECacheKeysInfo.DadosTemporarios, hash);
        var dados = cacheService.ObterCache<DadosTemporarios>(chave);

        if (dados.IsNullable())
            return Resultado.Falha(AuthResource.Erro_CacheExpiradoNaTrocaDeSenha);

        var senha = request.NovaSenha;
        var repetir = request.RepetirSenha;

        if (senha.IsNullOrEmpty() || repetir.IsNullOrEmpty())
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
