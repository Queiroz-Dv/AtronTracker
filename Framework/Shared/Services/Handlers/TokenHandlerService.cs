using Microsoft.IdentityModel.Tokens;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Interfaces.Handlers;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shared.Services.Handlers
{
    public class TokenHandlerService : ITokenHandlerService
    {
        private readonly IApplicationTokenService _tokenService;
        private readonly MessageModel _messageModel;

        public TokenHandlerService(IApplicationTokenService tokenService, MessageModel messageModel)
        {
            _tokenService = tokenService;
            _messageModel = messageModel;
        }

        public string ObterCodigoUsuarioPorClaim(string token)
        {
            var codigoUsuario = ParseToken(token).Claims.FirstOrDefault(c => c.Type == ClaimCode.CODIGO_USUARIO)?.Value; 

            if (codigoUsuario is null)
            {
                _messageModel.AddError("Token sem informações de usuário.");
                return null;
            }

            return codigoUsuario;
        }

        private ClaimsPrincipal ParseToken(string token)
        {
            var tokenValidationParameters = CreateTokenValidationParameters();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtToken = (JwtSecurityToken)securityToken;

                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _messageModel.AddError("Token inválido.");
                    return null;
                }

                return principal;
            }
            catch (Exception ex)
            {
                _messageModel.AddError(ex.Message);
                return null;
            }
        }

        private TokenValidationParameters CreateTokenValidationParameters()
        {
            var taskSecretKey = _tokenService.ObterChaveSecreta();
            taskSecretKey.Wait();
            var chaveSecreta = taskSecretKey.Result;

            var secretKey = Encoding.UTF8.GetBytes(chaveSecreta);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = false
            };
            return tokenValidationParameters;
        }

        public ClaimsPrincipal ObterClaimPrincipal(string token)
        {
            var claimsPrincipal = ParseToken(token);
            return claimsPrincipal;
        }
    }
}
