using Shared.Domain;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraPredicadoExtensions
    {
        public static Regra<T, TProp> DeveSer<T, TProp>(this Regra<T, TProp> regra, Func<TProp, bool> condicao)
        {
            if (condicao.IsNullable())
            {
                regra.Validacoes.PorValor.Add(_ => false);

                regra.MensagemErro ??=
                    $"A condição de validação do campo " +
                    $"{regra.Propriedade.NomePropriedade} " +
                    $"não foi informada.";

                return regra;
            }

            regra.Validacoes.PorValor.Add(condicao);

            return regra;
        }

        public static Regra<T, TProp> DeveSer<T, TProp>(this Regra<T, TProp> regra, Func<T, TProp, bool> condicao)
        {
            if (condicao.IsNullable())
            {
                regra.Validacoes.PorValor.Add(_ => false);

                regra.MensagemErro ??=
                    $"A condição de validação do campo " +
                    $"{regra.Propriedade.NomePropriedade} " +
                    $"não foi informada.";

                return regra;
            }

            regra.Validacoes.ComEntidade.Add(condicao);

            return regra;
        }
    }

}
