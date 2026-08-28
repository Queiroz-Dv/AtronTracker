using Shared.Application.DTOS.Users;
using Shared.Application.DTOS.Auth;
using Shared.Extensions;
using Xunit;

namespace Shared.Tests.Extensions;

public sealed class JwtConfigurationTests
{
    [Fact]
    public void GetClaims_DeveIncluirDadosDaEmpresaQuandoUsuarioEstiverVinculado()
    {
        var dados = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO { CodigoDoUsuario = "USR001" },
            DadosDaEmpresa = new DadosDaEmpresaDTO
            {
                Id = 12,
                Codigo = "ATR-EMP",
                NomeFantasia = "Atron Estudos"
            }
        };

        var claims = JwtConfiguration.GetClaims(dados);

        Assert.Equal("ATR-EMP", claims.Single(claim => claim.Type == ClaimCode.CODIGO_EMPRESA).Value);
        Assert.Equal("Atron Estudos", claims.Single(claim => claim.Type == ClaimCode.NOME_EMPRESA).Value);
    }

    [Fact]
    public void GetClaims_NaoDeveCriarClaimsEmpresariaisParaUsuarioSemVinculo()
    {
        var dados = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO { CodigoDoUsuario = "USR001" }
        };

        var claims = JwtConfiguration.GetClaims(dados);

        Assert.DoesNotContain(claims, claim => claim.Type == ClaimCode.CODIGO_EMPRESA);
        Assert.DoesNotContain(claims, claim => claim.Type == ClaimCode.NOME_EMPRESA);
    }
}
