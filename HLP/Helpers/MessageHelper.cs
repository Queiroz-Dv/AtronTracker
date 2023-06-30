using HLP.Enums;
using HLP.Interfaces;
using System;
using System.Windows.Forms;

namespace HLP.Helpers
{
    /// <summary>
    /// Classe abstrata que fornece métodos utilitários para exibir mensagens em caixas de diálogo.
    /// </summary>
    public  class MessageHelper : IMessageHelper
    {

        /// <summary>
        /// Exibe uma caixa de diálogo com uma mensagem de acordo com as condições fornecidas.
        /// </summary>
        /// <param name="error">A mensagem de erro a ser exibida.</param>
        /// <param name="buttons">Os botões da caixa de diálogo.</param>
        /// <param name="level">O nível da mensagem.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        public DialogResult ShowMessage(string error, MessageBoxButtons buttons, EnumLevelMessage level)
        {
            string message = GetLevelMessage(level);
            var icon = GetLevelIcon(level);
            return MessageBox.Show(error, message, buttons, icon);

            throw new Exception("Error In Information Message Layer");
        }

        private  MessageBoxIcon GetLevelIcon(EnumLevelMessage level)
        {
            switch (level)
            {
                case EnumLevelMessage.Critical: return MessageBoxIcon.Error;
                case EnumLevelMessage.Warning: return MessageBoxIcon.Warning;
                case EnumLevelMessage.Information: return MessageBoxIcon.Information;
                case EnumLevelMessage.Question: return MessageBoxIcon.Question;

                default: throw new ArgumentException("Invalid level");
            }
        }

        private  string GetLevelMessage(EnumLevelMessage level)
        {
            switch (level)
            {
                case EnumLevelMessage.Critical: return "Critical error";
                case EnumLevelMessage.Warning: return "Warning";
                case EnumLevelMessage.Information: return "Information";
                case EnumLevelMessage.Question: return "Question";

                default: throw new ArgumentException("Invalid level");
            }
        }
    }
}