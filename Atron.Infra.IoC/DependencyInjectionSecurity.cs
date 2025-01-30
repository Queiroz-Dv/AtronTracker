using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Services;
using System;
using System.Text;

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
