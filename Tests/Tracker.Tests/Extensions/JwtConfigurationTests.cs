using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Extensions;
using System.Linq;
using Xunit;

namespace Tracker.Tests.Extensions;

public class JwtConfigurationTests
{
    [Fact]
    public void JwtClaims_WorkspaceNull_NaoDeveLancarExcecao()
    {
        var dadosComplementares = new DadosComplementaresDoUsuarioDTO
        {
            DadosDoUsuario = new DadosDoUsuarioDTO
            {
                NomeDoUsuario = "Nome",
                Email = "email@teste.com",
                CodigoDoUsuario = "USR1",
                CodigoDoCargo = "CARG",
                CodigoDoDepartamento = "DEPT",
                Workspace = null!
            }
        };

        var claims = JwtConfiguration.GetClaims(dadosComplementares);

        Assert.NotNull(claims);
        var workspaceClaim = claims.FirstOrDefault(c => c.Type == ClaimCode.CODIGO_WORKSPACE);
        Assert.NotNull(workspaceClaim);
        Assert.Equal(string.Empty, workspaceClaim.Value);
    }
}
