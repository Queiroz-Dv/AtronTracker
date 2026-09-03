using Shared.Application.Records;
using Shared.Domain;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraCondicaoExtensions
    {
        public static Regra<T, TProp> Quando<T, TProp>(this Regra<T, TProp> regra, Func<T, bool> condicao)
        {
            if (condicao.IsNullable())
            {
                regra.MensagemErro ??=
                    $"A condição da regra do campo " +
                    $"{regra.Propriedade.NomePropriedade} não foi informada.";

                regra.Validacoes.PorValor.Add(_ => false);

                return regra;
            }

            regra.Condicao = new CondicaoRecord<T>(condicao);

            return regra;
        }

        public static Regra<T, TProp> Unless<T, TProp>(this Regra<T, TProp> regra, Func<T, bool> condicao)
        {
            if (condicao.IsNullable())
            {
                regra.MensagemErro ??=
                    $"A condição da regra do campo " +
                    $"{regra.Propriedade.NomePropriedade} não foi informada.";

                regra.Validacoes.PorValor.Add(_ => false);

                return regra;
            }

            regra.Condicao = new CondicaoRecord<T>(entidade => !condicao(entidade));

            return regra;
        }
    }
}