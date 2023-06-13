using MaterialSkin;
using MaterialSkin.Controls;
using System.Drawing;

namespace PersonalTracking
{
    public partial class FrmRegister : MaterialForm
    {
        public FrmRegister()
        {
            InitializeComponent();
            ConfigureCollorPallet();
            grpBasicInfo.BackColor = Color.FromArgb(24, 161, 251);
            grpUserInformation.BackColor = Color.FromArgb(49, 88, 155);
        }

        public void ConfigureCollorPallet()
        {
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
               Primary.DeepPurple900, Primary.DeepPurple500,
               Primary.Purple500, Accent.Purple200,
               TextShade.WHITE);
        }

    }
}
