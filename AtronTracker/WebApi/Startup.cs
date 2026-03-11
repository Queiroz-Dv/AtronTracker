using Domain.Interfaces.ApplicationInterfaces;
using IoC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Helpers;

namespace WebApi
{
    /// <summary>
    /// Classe principal onde são definidas as configurações e os serviços da API.
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

            // 🧱 Registra os serviços da camada de infraestrutura (ex: DbContext, Repositórios, JWT, AutoMapper, etc)
            services.AddInfrastructureAPI(Configuration);

            // 🔐 Registra os serviços necessários para a política dinâmica de autorização baseada em "módulo"
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicModuloPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, ModuloHandler>();
            // 🚀 Adiciona o suporte a Controllers (necessário para Web APIs)
            services.AddControllers();

            // 🌐 Permite injetar serviços HTTP (útil para chamadas externas)
            services.AddHttpClient();

            // 📎 Permite acessar o HttpContext em qualquer ponto via injeção de dependência
            services.AddHttpContextAccessor();

            // 📚 Adiciona o Swagger para documentação da API + filtro customizado para respostas
            services.AddSwaggerGen(c => c.OperationFilter<SwaggerResponseFilter>());
        }

        /// <summary>
        /// Método chamado na inicialização da aplicação para configurar o pipeline de requisições HTTP.
        /// </summary>
        /// <param name="app">Builder da aplicação.</param>
        /// <param name="env">Ambiente de hospedagem (dev, prod, etc).</param>
        /// <param name="createDefaultUserRole">Serviço que cria usuários e roles padrões na base de dados.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICreateDefaultUserRoleRepository createDefaultUserRole)
        {
            // 🐞 Ambiente de desenvolvimento: mostra tela de erro detalhada
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // 🔐 Ambiente de produção: força HTTPS via cabeçalho HSTS
                app.UseHsts(); // HTTP Strict Transport Security
            }

            // 🧾 Adiciona o Swagger para documentação da API
            AddSwagger(app);

            // 📘 Configura o ReDoc (interface alternativa ao Swagger UI)
            app.UseReDoc(c =>
            {
                c.RoutePrefix = "docs"; // Acessível em /docs
                c.DocumentTitle = "Atron WebApi Doc"; // Título da aba
                c.SpecUrl = "/swagger/v1/swagger.json"; // Localização do JSON de especificação
                c.ExpandResponses("200,201"); // Expande respostas 200 e 201 por padrão
            });

            // Cada método desse é um Middileware

            // 🔐 Redireciona todas as requisições HTTP para HTTPS automaticamente
            app.UseHttpsRedirection();

            // 📟 Exibe páginas amigáveis para códigos de erro HTTP (ex: 404, 500)
            app.UseStatusCodePages();

            // 🧭 Habilita o roteamento de requisições (fundamental para MapControllers)
            app.UseRouting();

            // 🧠 Cria roles e usuários padrões se ainda não existirem
           // createDefaultUserRole.CreateDefaultRoles();
         //   createDefaultUserRole.CreateDefaultUsers();

            // 🌍 Habilita a política de CORS para permitir requisições de outras origens (frontend, mobile, etc)
            app.UseCors("CorsPolicy");

            // 🔒 Ativa a autenticação (JWT ou qualquer outra configurada na infraestrutura)
            app.UseAuthentication();

            // 🔓 Ativa a autorização (necessário para aplicar `[Authorize]` nas rotas)
            app.UseAuthorization();

            // ♾️ Configura o ASP.NET Core para respeitar cabeçalhos X-Forwarded-* enviados por proxies reversos, como Nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // 🚀 Mapeia os endpoints das controllers para o pipeline de requisições
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Roteia para as controllers decoradas com [ApiController]
            });
        }

        /// <summary>
        /// Método auxiliar que registra o middleware de Swagger na aplicação.
        /// </summary>
        /// <param name="app">Application builder.</param>
        private static void AddSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(); // Gera o JSON com a especificação da API
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron WebApi Doc v1")); // Interface Swagger UI
        }
    }
}
