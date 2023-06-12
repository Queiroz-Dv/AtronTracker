using FontAwesome.Sharp;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmMainMenuView : Form
    {
        private IconButton currentButton;
        private Panel leftBorderBtn;
        private Form currentChildForm;

        public FrmMainMenuView()
        {
            InitializeComponent();
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            pnlMainMenuView.Controls.Add(leftBorderBtn);

            // Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
        }

        struct RGBColors
        {
            public static Color pallet1 = Color.FromArgb(172, 126, 241);
            public static Color pallet2 = Color.FromArgb(249, 118, 176);
            public static Color pallet3 = Color.FromArgb(253, 138, 114);
            public static Color pallet4 = Color.FromArgb(95, 77, 221);
            public static Color pallet5 = Color.FromArgb(49, 88, 155);
            public static Color pallet6 = Color.FromArgb(24, 161, 251);
        }

        private void ActivateButton(object senderButton, Color color)
        {
            if (senderButton != null)
            {
                DisableButton();
                // Button
                currentButton = senderButton as IconButton;
                currentButton.BackColor = Color.FromArgb(37, 36, 81);
                currentButton.ForeColor = color;
                currentButton.TextAlign = ContentAlignment.MiddleLeft;
                currentButton.IconColor = color;
                currentButton.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentButton.ImageAlign = ContentAlignment.MiddleRight;

                // Left Border Button
                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentButton.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();

                // Icon Child Form
                iconCurrentChildForm.IconChar = currentButton.IconChar;
                iconCurrentChildForm.IconColor = color;
            }
        }

        private void DisableButton()
        {
            if (currentButton != null)
            {
                currentButton.BackColor = Color.FromArgb(31, 30, 68);
                currentButton.ForeColor = Color.Gainsboro;
                currentButton.TextAlign = ContentAlignment.MiddleLeft;
                currentButton.IconColor = Color.Gainsboro;
                currentButton.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentButton.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }

            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlDesktop.Controls.Add(childForm);
            pnlDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitleChildForm.Text = childForm.Text;
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet1);
            OpenChildForm(new FrmEmployeeList());
        }

        private void btnDepartments_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet2);
            OpenChildForm(new FrmDepartmentList());
        }

        private void btnPosition_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet3);
            OpenChildForm(new FrmPositionList());
        }

        private void btnTask_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet4);
            OpenChildForm(new FrmTaskList());
        }

        private void btnSalary_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet5);
            OpenChildForm(new FrmSalaryList());
        }

        private void btnPermission_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.pallet6);
            OpenChildForm(new FrmSalaryList());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            DisableButton();
            leftBorderBtn.Visible = false;
            iconCurrentChildForm.IconChar = IconChar.Home;
            iconCurrentChildForm.IconColor = Color.MediumPurple;
            lblTitleChildForm.Text = "Home";
        }

        //Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void pnlTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
