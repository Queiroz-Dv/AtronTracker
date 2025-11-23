using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System;

namespace Application.Services.AuthServices.Bases
{
    public abstract class ServiceBase
    {
        protected readonly IAccessorService _accessor;

        protected ServiceBase(IAccessorService accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        // 🔐 Método central de resolução segura
        protected T ObterService<T>() where T : class
        {
            var instance = _accessor.ObterService<T>();
            if (instance is null)
                throw new InvalidOperationException($"Serviço de tipo {typeof(T).Name} não encontrado no container.");
            return instance;
        }

        // 🧭 Mensagens (centralizadas)
        protected Notifiable Messages => ObterService<Notifiable>();

        // 📏 Validação tipada por DTO
        protected IValidateModelService<T> GetValidator<T>() => ObterService<IValidateModelService<T>>();
    }
}
