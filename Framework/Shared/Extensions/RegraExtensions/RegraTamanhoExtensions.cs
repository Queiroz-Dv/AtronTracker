using Shared.Application.Resources;
using Shared.Domain;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraTamanhoExtensions
    {
        public static Regra<T, TProp> TamanhoMaiorQue<T, TProp>(this Regra<T, TProp> regra, int tamanho)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string texto)
                    return true;

                return texto.Trim().Length > tamanho;
            });

            return regra;
        }

        public static Regra<T, TProp> TamanhoMaiorOuIgualA<T, TProp>(this Regra<T, TProp> regra, int tamanho)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string texto)
                    return true;

                return texto.Trim().Length >= tamanho;
            });

            return regra;
        }

        public static Regra<T, TProp> TamanhoMenorQue<T, TProp>(this Regra<T, TProp> regra, int tamanho)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string texto)
                    return true;

                return texto.Trim().Length < tamanho;
            });

            return regra;
        }

        public static Regra<T, TProp> TamanhoMenorOuIgualA<T, TProp>(this Regra<T, TProp> regra, int tamanho)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string texto)
                    return true;

                return texto.Trim().Length <= tamanho;
            });

            return regra;
        }

        public static Regra<T, TProp> TamanhoEntre<T, TProp>(this Regra<T, TProp> regra, int minimo, int maximo)
        {
            if (minimo < 0 || maximo < minimo)
            {
                regra.Validacoes.PorValor.Add(_ => false);

                regra.MensagemErro ??=
                    string.Format(
                        NotificacoesPadronizadas.IntervaloDeTamanhoInvalido,
                        regra.Propriedade.NomePropriedade);

                return regra;
            }

            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string texto)
                    return true;

                var tamanho = texto.Trim().Length;

                return tamanho >= minimo &&
                       tamanho <= maximo;
            });

            return regra;
        }
    }

}