using BLL;
using DAL;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows;

namespace PersonalTracking
{
    public partial class FrmRegister : MaterialForm
    {
        public FrmRegister()
        {
            InitializeComponent();
            ConfigureCollorPallet();
            grpBasicInfo.BackColor = Color.Gainsboro;
            grpUserInformation.BackColor = Color.Gainsboro;
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

        private void txtUserNo_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()) ||
                string.IsNullOrEmpty(txtSurname.Text.Trim()))
            {
                MessageBox.Show($"{lblName.Text} or {lblSurname.Text} is empty");
            }

            if (string.IsNullOrEmpty(txtUserNo.Text.Trim()) ||
                string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                MessageBox.Show($"{lblUserNumber} or {lblPassword} is empty");
            }

            else
            {
                if (!EmployeeBLL.isUnique(Convert.ToInt32(txtUserNo.Text)))
                {
                    MessageBox.Show("This user is used by another employee please change it");
                }
                else
                {
                    EMPLOYEE employee = new EMPLOYEE
                    {
                        Name = txtName.Text,
                        Surname = txtSurname.Text,
                        UserNo = Convert.ToInt32(txtUserNo.Text),
                        Password = txtPassword.Text
                    };

                    EmployeeBLL.AddEmployee(employee);
                    ClearFields();
                }
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            txtPassword.Clear();
        }

        bool isUnique = false; 

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (txtUserNo.Text.Trim() == "")
                MessageBox.Show("User Number is Empty");
            else
            {
                isUnique = EmployeeBLL.isUnique(Convert.ToInt32(txtUserNo.Text));
                if (!isUnique)
                    MessageBox.Show("This user is used by another employee please change it");
                else
                {
                    MessageBox.Show("This user is usable");
                    InformationIsFilled(txtName.Text, txtSurname.Text, txtUserNo.Text, txtPassword.Text);
                }

            }
        }

        private void FrmRegister_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
        }

        private bool InformationIsFilled(string name, string surname, string userNo, string password)
        {
            if (name.Trim().Length > 3 && 
                surname.Trim().Length > 3 || 
                userNo.Trim().Length > 1 && password.Length > 8)
            {
                 
                return btnSave.Enabled = true;
            }
            else
            {
                return btnSave.Enabled = false;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
