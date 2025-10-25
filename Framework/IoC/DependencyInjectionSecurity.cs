using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using System;
using System.Text;
using System.Text.Json;

namespace IoC
{
    public static class DependencyInjectionSecurity
    {
        public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration.GetSecretKey();
            var issueSigniKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

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

                    OnAuthenticationFailed = async context =>
                    {
                        // Optional: log, mas não escreva no response aqui
                        context.NoResult(); // Cancela a resposta padrão
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new { status = 401, message = "Token inválido ou ausente." })
                        );
                    },
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

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