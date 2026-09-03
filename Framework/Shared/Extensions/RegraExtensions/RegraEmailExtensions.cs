using Shared.Domain;
using System.Net.Mail;

namespace Shared.Extensions.RegraExtensions
{
    public static class RegraEmailExtensions
    {
        public static Regra<T, TProp> EmailValido<T, TProp>(
            this Regra<T, TProp> regra)
        {
            regra.Validacoes.PorValor.Add(valor =>
            {
                if (valor.IsNullable())
                    return true;

                if (valor is not string email)
                    return true;

                if (email.IsNullOrEmpty())
                    return true;

                var tratado = email.Trim();

                return MailAddress.TryCreate(tratado, out var mail)
                       && mail.Address.IsEquals(tratado);
            });

            return regra;
        }
    }
}