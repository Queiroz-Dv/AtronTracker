﻿using System.ComponentModel;

namespace Shared.Enums
{
    /// <summary>
    /// Enumerador de nível da mensagem
    /// </summary>
    [Serializable]
    public enum ResultResponseLevelEnum
    {
        [Description("Message")]
        Message,

        [Description("Error")]
        Error,

        [Description("Warning")]
        Warning
    }
}