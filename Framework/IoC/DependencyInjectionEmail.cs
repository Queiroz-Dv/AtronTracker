using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Email;
using Shared.Application.Validacoes;

namespace IoC
{
    public static class DependencyInjectionEmail
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, SharedEmailService>();
            services.AddScoped<IValidador<EmailRequest>, EmailValidador>();
            services.AddScoped<IEmailDiagnosticService, EmailDiagnosticService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            return services;
        }
    }
}
