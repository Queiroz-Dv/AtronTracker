using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Net.Mail;

namespace Shared.Extensions
{
    public static class NotificationBagExtensions
    {
        /// <summary>
        /// Valida se um campo não é nulo/vazio e tem comprimento dentro do intervalo especificado.
        /// Adiciona mensagens de erro ao NotificationBag.
        /// </summary>
        public static void ValidarCampo(this NotificationBag bag, string? valor, int tamanhoMinimo, int tamanhoMaximo, string nomeCampo)
        {
            if (valor.IsNullOrEmpty())
            {
                bag.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nomeCampo));
                return;
            }

            var tamanho = valor.Trim().Length;
            if (tamanho < tamanhoMinimo || tamanho > tamanhoMaximo)
            {
                bag.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoInvalido, nomeCampo));
            }
        }

        /// <summary>
        /// Sobrecarga para quando não há validação de tamanho (apenas obrigatoriedade).
        /// </summary>
        public static void ValidarCampoObrigatorio(this NotificationBag bag, string? valor, string nomeCampo)
        {
            if (valor.IsNullOrEmpty())
                bag.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoObrigatorio, nomeCampo));
        }

        /// <summary>
        /// Valida se o valor é um e-mail válido (opcional).
        /// </summary>
        public static void ValidarEmail(
            this NotificationBag bag,
            string? email,
            string nomeCampo)
        {
            if (email.IsNullOrEmpty())
                return;

            if (!MailAddress.TryCreate(email.Trim(), out var mail) ||
                !mail.Address.IsEquals(email.Trim()))
            {
                bag.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoInvalido, nomeCampo));
            }
        }

        public static void ValidarEnumeracao<TEnum>(this NotificationBag bag, TEnum valor, string nomeCampo) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(valor))
            {
                bag.AdicionarErro(string.Format(NotificacoesPadronizadas.ErroCampoInvalido, nomeCampo));
            }
        }
    }
}