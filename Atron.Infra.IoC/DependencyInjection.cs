using Atron.Application.Interfaces;
using Atron.Application.Mapping;
using Atron.Application.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Interfaces;
using Notification.Models;
using Notification.Services;

namespace Atron.Infra.IoC
{
    /// <summary>
    /// Classe responsável pela injeção de dependência
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // O método AddScoped indica que os serviços são criados uma vez por requisição HTTP
            // O método Singleton indica que o serviço é criado uma vez para todas as requisições
            // O método Transiente indica que sempre será criado um novo serviço cada vez que for necessário

            // Como padrão vou manter o AddScoped pois atende melhor a aplicação com um todo 

            services.AddDbContext<AtronDbContext>(options =>
            // Define o provedor e a string de conexão
            options.UseSqlServer(configuration.GetConnectionString("AtronConnection"),
            // Define o asembly de onde as migrações devem ser mantidas 
            m => m.MigrationsAssembly(typeof(AtronDbContext).Assembly.FullName)));

            // Registra os repositories
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<ICargoRepository, CargoRepository>();
            //services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Registra os Serviços
            services.AddScoped<IDepartamentoService, DepartamentoService>();
            services.AddScoped<ICargoService, CargoService>();

            // Serviços utilitários 
            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));
            services.AddScoped<NotificationService, NotificationModel<Departamento>>();

            return services;
        }
    }
}