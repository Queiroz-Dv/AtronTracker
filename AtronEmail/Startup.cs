using IoC;
using Microsoft.AspNetCore.HttpOverrides;

namespace AtronEmail
{
    /// <summary>
    /// Classe principal onde são definidas as configurações e os serviços da API de E-mail.
    /// Responsável por configurar a injeção de dependência e o pipeline de requisições.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Construtor da classe Startup. Recebe a configuração da aplicação via injeção de dependência.
        /// </summary>
        /// <param name="configuration">Configuração da aplicação.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Propriedade que expõe a configuração da aplicação (appsettings.json, variáveis de ambiente, etc).
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Método chamado pela aplicação para registrar os serviços (injeção de dependência).
        /// </summary>
        /// <param name="services">Contêiner de serviços.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddEmailServices(Configuration);
            services.AddControllers();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Atron Email API",
                    Version = "v1",
                    Description = "Módulo de serviços de e-mail do Sistema Atron"
                });
            });
        }

        /// <summary>
        /// Método chamado na inicialização da aplicação para configurar o pipeline de requisições HTTP.
        /// </summary>
        /// <param name="app">Builder da aplicação.</param>
        /// <param name="env">Ambiente de hospedagem (dev, prod, etc).</param>
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

            // Swagger
            AddSwagger(app);

            // ReDoc
            app.UseReDoc(c =>
            {
                c.RoutePrefix = "docs";
                c.DocumentTitle = "Atron Email Doc";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.ExpandResponses("200,201");
            });

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Método auxiliar que registra o middleware de Swagger na aplicação.
        /// </summary>
        /// <param name="app">Application builder.</param>
        private static void AddSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron Email API v1"));
        }
    }
}
