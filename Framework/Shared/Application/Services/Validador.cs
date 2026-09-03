using Shared.Application.Interfaces.Service;
using Shared.Domain;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Services
{
    public abstract class Validador<T> : IValidador<T>
    {
        private readonly List<Func<T, NotificationMessage?>> _regras = [];

        protected Regra<T, TProp> RegraPara<TProp>(Func<T, TProp> propriedade)
        {
            if (propriedade is null)
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
                bag.AdicionarErro("O registro informado para validação é nulo.");
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