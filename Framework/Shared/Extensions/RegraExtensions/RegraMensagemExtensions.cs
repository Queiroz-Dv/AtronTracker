using Shared.Domain;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraMensagemExtensions
    {
        public static Regra<T, TProp> ComMensagem<T, TProp>(
            this Regra<T, TProp> regra,
            string mensagem)
        {
            if (mensagem.IsNullOrEmpty())
            {
                regra.Validacoes.PorValor.Add(_ => false);

                regra.MensagemErro ??=
                    $"A mensagem da regra do campo " +
                    $"{regra.Propriedade.NomePropriedade} não foi informada.";

                return regra;
            }

            regra.MensagemErro = mensagem;

            return regra;
        }
    }
}