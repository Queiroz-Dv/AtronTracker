using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class DependencyInjectionCookieContainer
    {
        public static IServiceCollection AddCustomCookieConfiguration(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/";
                options.LoginPath = "/";
                options.LogoutPath = "/";             

                options.Cookie = new CookieBuilder
                {
                    Name = "AuthCookie",
                    HttpOnly = true,
                    SecurePolicy = CookieSecurePolicy.Always,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                };

            });

            services.AddSession(options =>
            {
                options.Cookie.Name = "AuthSession";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            return services;
        }
    }
}
