using Shared.Domain;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraValidacaoExtensions
    {
        public static Regra<T, TProp> NaoVazio<T, TProp>(this Regra<T, TProp> regra)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return false;

                if (valor is string texto)
                    return !texto.IsNullOrEmpty();

                return true;
            });

            return regra;
        }
    }
}