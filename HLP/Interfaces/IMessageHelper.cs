using HLP.Enums;
using System.Windows.Forms;

namespace HLP.Interfaces
{
    public interface IMessageHelper
    {
        DialogResult ShowMessage(string error, MessageBoxButtons buttons, EnumLevelMessage level);
    }
}
