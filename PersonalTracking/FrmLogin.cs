using BLL;
using DAL;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmLogin : MaterialForm
    {
        public FrmLogin()
        {
            InitializeComponent();
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
               Primary.DeepPurple900, Primary.DeepPurple500,
               Primary.Purple500, Accent.Purple200,
               TextShade.WHITE
           );
        }

        private void txtUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Verifica se está vazio
            if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "")
                MessageBox.Show("Please fill the User Numb and Password");
            else
            {
                //Se estiver preenchido mas o usuário não existe ele exibe um erro
                List<EMPLOYEE> employeelist = EmployeeBLL.GetEmployees(Convert.ToInt32(txtUserNo.Text), txtPassword.Text);
                if (employeelist.Count == 0)
                    MessageBox.Show("Please control your information");
                else
                {
                    //Se existir ele pega o primeiro usuário e o mantém na sessão ativa e abre o menu
                    EMPLOYEE employee = new EMPLOYEE();
                    employee = employeelist.First();
                    UserStatic.EmployeeID = employee.ID;
                    UserStatic.UserNo = employee.UserNo;
                    UserStatic.isAdmin = Convert.ToBoolean(employee.isAdmin);
                    FrmMain frm = new FrmMain();
                    this.Hide();
                    frm.ShowDialog();
                }
            }
        }

        private void btnLogin_MouseHover(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.BlanchedAlmond;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.White;
        }

        private void btnExit_MouseHover(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.BlanchedAlmond;
        }

        private void btnExit_MouseLeave(object sender, EventArgs e)
        {
            btnExit.BackColor = Color.White;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "")
                    MessageBox.Show("Please fill the User Numb and Password");
                else
                {
                    List<EMPLOYEE> employeelist = EmployeeBLL.GetEmployees(Convert.ToInt32(txtUserNo.Text), txtPassword.Text);
                    if (employeelist.Count == 0)
                        MessageBox.Show("Please control your information");
                    else
                    {
                        EMPLOYEE employee = new EMPLOYEE();
                        employee = employeelist.First();
                        UserStatic.EmployeeID = employee.ID;
                        UserStatic.UserNo = employee.UserNo;
                        UserStatic.isAdmin = Convert.ToBoolean(employee.isAdmin);
                        FrmMain frm = new FrmMain();
                        this.Hide();
                        frm.ShowDialog();
                    }
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            FrmRegister frm = new FrmRegister();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }
    }
}
