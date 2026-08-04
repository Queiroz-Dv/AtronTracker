using AtronNotificacoes.Infrastructure.DependencyInjection;
using AtronStock.Infrastructure;
using AtronPlatform.WebApi.OpenApi;
using AtronPlatform.WebApi.Security;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Shared.Infrastructure.DependencyInjection;
using Infrastructure.DependencyInjection;

namespace AtronPlatform.WebApi;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAtronCache(Configuration);
        services.AddSharedInfrastructure(Configuration);
        services.AddAtronApiDocumentation();
        services.AddEmailServices(Configuration);
        services.AddNotificacoesInternasCapability(Configuration);
        services.AddTrackerModule(Configuration);
        services.AddStockModule(Configuration);
        services.AddInfrastructureSecurity(Configuration);
        services.AddAcessoRateLimiting();
        services.AddControllers();
        services.AddHealthChecks();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(provider =>
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Response.Cookies
            ?? throw new InvalidOperationException(
                "Os cookies de resposta somente estão disponíveis durante uma requisição HTTP."));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        if (environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron Platform API v1"));
            app.UseReDoc(options =>
            {
                options.RoutePrefix = "docs";
                options.DocumentTitle = "Atron Platform Doc";
                options.SpecUrl = "/swagger/v1/swagger.json";
                options.ExpandResponses("200,201");
            });
        }
        else
        {
            // Força o uso de HTTPS e HSTS em produção
            app.UseCabecalhosSeguranca();
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStatusCodePages();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/api/saude", new HealthCheckOptions
            {
                Predicate = registration =>
                    !registration.Tags.Contains(
                        NotificacoesInternasServiceCollectionExtensions.TagProntidao)
            }).AllowAnonymous();
            endpoints.MapHealthChecks("/api/notificacoes/saude", new HealthCheckOptions
            {
                Predicate = registration =>
                    registration.Tags.Contains(
                        NotificacoesInternasServiceCollectionExtensions.TagProntidao)
            }).RequireAuthorization();
        });
    }
}
