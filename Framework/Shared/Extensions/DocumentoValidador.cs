using System.Text.RegularExpressions;

namespace Shared.Extensions
{
    /// <summary>
    /// Fornece métodos estáticos para validar formatos de documentos.
    /// É uma função pura, sem estado.
    /// </summary>
    public static class DocumentoValidator
    {
        // Expressões regulares para CPF e CNPJ (exemplos simples, podem ser melhorados)
        private static readonly Regex CpfRegex = new Regex(@"^\d{11}$");
        private static readonly Regex CnpjRegex = new Regex(@"^\d{14}$");

        /// <summary>
        /// Valida um número de CPF (apenas formato, não o dígito verificador).
        /// </summary>
        public static bool IsValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres comuns de formatação
            var cpfLimpo = cpf.Trim().Replace(".", "").Replace("-", "");

            // Valida o formato e alguns casos inválidos conhecidos
            if (!CpfRegex.IsMatch(cpfLimpo) || cpfLimpo == "00000000000")
                return false;

            // TODO: Adicionar a lógica do dígito verificador aqui
            return true;
        }

        /// <summary>
        /// Valida um número de CNPJ (apenas formato, não o dígito verificador).
        /// </summary>
        public static bool IsValidCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            var cnpjLimpo = cnpj.Trim().Replace(".", "").Replace("/", "").Replace("-", "");

            if (!CnpjRegex.IsMatch(cnpjLimpo) || cnpjLimpo == "00000000000000")
                return false;

            // TODO: Adicionar a lógica do dígito verificador aqui
            return true;
        }
    }
}
