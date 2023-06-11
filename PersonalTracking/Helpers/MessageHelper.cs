using System;
using System.Windows.Forms;

namespace PersonalTracking.Helpers
{
    public abstract class MessageHelper
    {
        public enum EnumLevelMessage
        {
            Critical,
            Warning,
            Information,
            Question
        }

        public static DialogResult ShowMessage(bool condition, string error, MessageBoxButtons buttons, EnumLevelMessage level)
        {
            if (condition)
            {
                string message = GetLevelMessage(level);
                var icon = GetLevelIcon(level);
                return MessageBox.Show(error, message, buttons, icon);
            }

            throw new Exception("Error In Information Message Layer");
        }

        private static MessageBoxIcon GetLevelIcon(EnumLevelMessage level)
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

        private static string GetLevelMessage(EnumLevelMessage level)
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