using Helpers.Enums;
using System.Windows.Forms;

namespace Helpers.Interfaces
{
    public interface IMessageHelper
    {
        DialogResult ShowMessage(string error, MessageBoxButtons buttons, EnumLevelMessage level);
    }
}
