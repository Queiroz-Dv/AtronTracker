using Shared.Application.Interfaces.Service;
using Shared.Domain;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using Shared.Application.Resources;

namespace Shared.Application.Services
{
    public abstract class Validador<T> : IValidador<T>
    {
        private readonly List<Func<T, NotificationMessage?>> _regras = new();

        protected Regra<T, TProp> RegraPara<TProp>(Func<T, TProp> propriedade)
        {
            if (propriedade.IsNullable())
                return new Regra<T, TProp>(_ => default!, "Propriedade");

            var regra = new Regra<T, TProp>(propriedade, "Campo");
            _regras.Add(regra.Executar);
            return regra;
        }

        public NotificationBag Validar(T entity)
        {
            var bag = new NotificationBag();

            if (entity is null)
            {
                bag.AdicionarErro(NotificacoesPadronizadas.ErroRegistroNulo);
                return bag;
            }

            foreach (var regra in _regras)
            {
                var mensagem = regra(entity);
                if (mensagem is not null)
                    bag.Adicionar(mensagem);
            }

            return bag;
        }

        IEnumerable<NotificationMessage> IValidador<T>.Validar(T entity)
            => Validar(entity).Messages;
    }
}