using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Application.Resources;
using Shared.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.DependencyInjection
{
    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var secretKey = configuration.GetSecretKey();

            // If no secret is configured, fall back to a generated temporary key in Development only.
            // This avoids startup failure in local dev, but prevents accidental use in production.
            byte[] keyBytes;
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("JWT signing secret is not configured. Set the secret (e.g. Jwt:Secret) in configuration.");

                // Development fallback: generate a 256-bit random key
                keyBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(32);
            }
            else
            {
                keyBytes = Encoding.UTF8.GetBytes(secretKey);
            }

            var issueSigniKey = new SymmetricSecurityKey(keyBytes);

            services.AddCors(options =>
            {
                var allowedOrigins = configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>()
                    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
                    .ToArray();

                if (allowedOrigins is null || allowedOrigins.Length == 0)
                    allowedOrigins = new[] { "http://localhost:4200" };

                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var result = JsonSerializer.Serialize(new
                        {
                            status = 403,
                            message = AuthResource.Erro_AcessoNegado
                        });
                        return context.Response.WriteAsync(result);
                    },

                    OnAuthenticationFailed = async context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new
                            {
                                status = 401,
                                message = AuthResource.Erro_TokenInvalidoOuAusente
                            })
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
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
