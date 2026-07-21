using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AtronNotificacoes.Application;
using AtronNotificacoes.Domain;
using AtronNotificacoes.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AtronNotificacoes;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        ConfigureCors(services);
        services.AddHttpContextAccessor();
        ConfigureJwtAuthentication(services);
        ConfigureAuthorization(services);
        ConfigureNotificacoesPersistence(services);
        services.AddHealthChecks()
            .AddCheck<ProntidaoBancoNotificacoesCheck>("banco-notificacoes", tags: new[] { "ready" });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Atron Notificações API",
                Version = "v1",
                Description = "Módulo transversal de notificações internas da plataforma Atron."
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron Notificações API v1"));
        app.UseReDoc(options =>
        {
            options.RoutePrefix = "docs";
            options.DocumentTitle = "Atron Notificações";
            options.SpecUrl = "/swagger/v1/swagger.json";
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("Angular");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/api/notificacoes/saude")
                .RequireAuthorization(SegurancaNotificacoes.PoliticaPublicador);
        });
    }

    private void ConfigureJwtAuthentication(IServiceCollection services)
    {
        var secretKeyUsuario = Configuration["Jwt:SecretKey"];
        var secretKeyServico = Configuration["Servico:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKeyUsuario) || string.IsNullOrWhiteSpace(secretKeyServico))
        {
            throw new InvalidOperationException("Jwt:SecretKey deve ser configurada para iniciar o módulo de notificações.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = new[] { Configuration["Jwt:Issuer"], Configuration["Servico:Issuer"] },
                    ValidAudiences = new[] { Configuration["Jwt:Audience"], Configuration["Servico:Audience"] },
                    IssuerSigningKeys = new[]
                    {
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyUsuario)),
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyServico))
                    },
                    ClockSkew = TimeSpan.Zero
                };
            });
    }

    private void ConfigureCors(IServiceCollection services)
    {
        var origens = Configuration.GetSection("Cors:AllowedOrigins")
            .Get<string[]>()?
            .Where(origem => !string.IsNullOrWhiteSpace(origem))
            .ToArray() ?? [];

        if (origens.Length == 0)
            throw new InvalidOperationException("Cors:AllowedOrigins deve conter as origens do Angular.");

        services.AddCors(options => options.AddPolicy("Angular", policy => policy
            .WithOrigins(origens)
            .AllowAnyHeader()
            .AllowAnyMethod()));
    }

    private static void ConfigureAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(SegurancaNotificacoes.PoliticaUsuario, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireAssertion(context =>
                        context.User.HasClaim(claim =>
                            claim.Type == SegurancaNotificacoes.ClaimCodigoUsuario &&
                            !string.IsNullOrWhiteSpace(claim.Value)) &&
                        !context.User.HasClaim(
                            SegurancaNotificacoes.ClaimTipoToken,
                            SegurancaNotificacoes.TipoTokenServico)));

            options.AddPolicy(SegurancaNotificacoes.PoliticaPublicador, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(SegurancaNotificacoes.ClaimTipoToken, SegurancaNotificacoes.TipoTokenServico)
                    .RequireClaim(SegurancaNotificacoes.ClaimEscopo, SegurancaNotificacoes.EscopoPublicar));
        });
    }

    private void ConfigureNotificacoesPersistence(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection deve ser configurada para iniciar o módulo de notificações.");
        }

        services.AddDbContext<NotificacoesDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql
                    .MigrationsAssembly("AtronNotificacoes.Infrastructure")
                    .MigrationsHistoryTable("__AtronNotificacoesMigrationsHistory")));
        services.AddScoped<INotificacaoInternaRepository, NotificacaoInternaRepository>();
        services.AddScoped<INotificacaoInternaService, NotificacaoInternaService>();
    }
}
