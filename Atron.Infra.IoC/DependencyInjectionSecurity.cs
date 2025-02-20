using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Services;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionSecurity
    {
        public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration.GetSecretKey();
            var issueSigniKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddScoped<IApplicationTokenService, ApplicationTokenService>();

            // informar o tipo de autenticacao JWTBearer    
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Validar token
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {                   
                    OnChallenge = context =>
                    {
                        // Personaliza a resposta quando o token é inválido ou ausente
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var result = JsonSerializer.Serialize(new
                        {
                            status = 401,
                            message = "Token inválido ou ausente."
                        });
                        context.Response.Headers["Location"] = "ApplicationLogin/Login"; // Redireciona para a página de Login                        
                        return context.Response.WriteAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        // Personaliza a resposta quando o usuário está autenticado, mas não tem permissão para acessar o recurso
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var result = JsonSerializer.Serialize(new
                        {
                            status = 403,
                            message = "Acesso negado. Você não tem permissão para acessar este recurso."
                        });
                        return context.Response.WriteAsync(result);
                    },

                    OnAuthenticationFailed = context =>
                    {
                        // Personaliza a resposta quando ocorre uma falha na autenticação, como erro ao validar o token
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var result = JsonSerializer.Serialize(new
                        {
                            status = 500,
                            message = "Erro interno na autenticação. Verifique o token enviado."
                        });
                        return context.Response.WriteAsync(result);
                    }
                };                

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration.GetIssuer(),
                    ValidAudience = configuration.GetAudience(),
                    IssuerSigningKey = issueSigniKey,
                    ClockSkew = TimeSpan.Zero // Zerando os cinco minutos de tempo de vida do token
                };
            });

            return services;
        }
    }
}