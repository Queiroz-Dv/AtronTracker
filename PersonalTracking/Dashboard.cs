using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDashboard : Form
    {
        private Form currentForm;

        public FrmDashboard()
        {
            InitializeComponent();

            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
        }

        public void OpenChildForm(Form childForm)
        {
            if (currentForm != null)
            {
                currentForm.Close();
            }

            currentForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlDashBoard.Controls.Add(childForm);
            pnlDashBoard.Tag = childForm;
            //childForm.Size = new Size(1022, 823);
            childForm.BringToFront();
            childForm.Show();
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmEmployeeList());
        }

        private void btnDepartments_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmDepartmentList());
        }


        private void btnPosition_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmPositionList());
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnTask_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmTaskList());
        }

        private void btnSalary_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmSalaryList());
        }

        private void btnPermission_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FrmPermissionList());
        }
    }
}
