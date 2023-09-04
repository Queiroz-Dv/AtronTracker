using PersonalTracking.Helper.Enums;
using System.Windows.Forms;

namespace PersonalTracking.Helper.Interfaces
{
    public interface IMessageHelper
    {
        DialogResult ShowMessage(string error, MessageBoxButtons buttons, EnumLevelMessage level);
    }
}
