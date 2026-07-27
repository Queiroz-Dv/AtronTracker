using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Email;
using Shared.Application.Validacoes;

namespace Shared.Infrastructure.DependencyInjection
{
    public static class EmailServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, SharedEmailService>();
            services.AddScoped<IValidador<EmailRequest>, EmailValidador>();
            services.AddScoped<IEmailDiagnosticService, EmailDiagnosticService>();
            services.AddSingleton<IEmailTemplateRenderer, EmailTemplateRenderer>();
            return services;
        }
    }
}
