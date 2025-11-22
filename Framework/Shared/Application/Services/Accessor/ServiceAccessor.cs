using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services.Accessor
{
    public class ServiceAccessor : IAccessorService
    {
        private readonly IServiceProvider _provider;
        private readonly Dictionary<Type, object> _cache = new();

        public ServiceAccessor(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T ObterService<T>() where T : class
        {
            var tipo = typeof(T);

            if (_cache.ContainsKey(tipo))
                return _cache[tipo] as T;

            var resolved = _provider.GetService(tipo) as T;

            if (resolved is not null)
            {
                _cache[tipo] = resolved;
                return resolved;
            }

            throw new InvalidOperationException($"Serviço do tipo {tipo.Name} não está registrado no container.");
        }
    }
}