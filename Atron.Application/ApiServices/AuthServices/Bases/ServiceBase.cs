using Shared.Interfaces.Accessor;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;

namespace Atron.Application.ApiServices.AuthServices.Bases
{
    public abstract class ServiceBase
    {
        protected readonly IServiceAccessor _accessor;

        protected ServiceBase(IServiceAccessor accessor)
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
        protected MessageModel Messages => ObterService<MessageModel>();

        // 📏 Validação tipada por DTO
        protected IValidateModel<T> GetValidator<T>() => ObterService<IValidateModel<T>>();
    }
}
