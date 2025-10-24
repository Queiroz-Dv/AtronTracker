using Atron.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Atron.Tracker.WebApi.Helpers
{
    public class DynamicModuloPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        const string Prefixo = "Modulo:";
        //private readonly AuthorizationOptions _options;
        public DynamicModuloPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
           // _options = options.Value;
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string name)
        {
            if (name.StartsWith(Prefixo, StringComparison.OrdinalIgnoreCase))
            {
                var code = name[Prefixo.Length..];
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ModuloRequirement(code))
                    .Build();
                return Task.FromResult(policy);
            }
            return base.GetPolicyAsync(name);
        }
    }
}
