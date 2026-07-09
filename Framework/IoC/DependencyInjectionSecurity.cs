using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Extensions;
using System;
using System.Linq;
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
                            message = "Acesso negado. Você não tem permissão para acessar este recurso."
                        });
                        return context.Response.WriteAsync(result);
                    },

                    OnAuthenticationFailed = async context =>
                    {
                        context.NoResult();
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
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
