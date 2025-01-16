using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionApiDoc
    {
        public static void AddDependencyInjectionApiDoc(this IServiceCollection services)
        {
            // Informa que usaremos o Swagger para documentação e testes
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Atron API",
                    Version = "v1",
                    Description = "Uma API desenvolvida por E. Queiroz para estudos e testes",
                    Contact = new OpenApiContact() { Name = "Eduardo Queiroz", Email = "queiroz.dv@outlook.com" }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,

                    Description = "Definições de segurança JWT."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                c.EnableAnnotations();
            });
        }
    }
}
