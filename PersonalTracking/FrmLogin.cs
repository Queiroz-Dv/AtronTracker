using BLL.Interfaces;
using DAL;
using HLP.Entity;
using HLP.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmLogin : Form, IValidateHelper
    {
        private readonly IEntityMessages Information;
        private readonly IEmployeeService _employeeService;
        private IDepartmentService _departmentService;

        public FrmLogin(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            InitializeComponent();
            Information = new InformationMessage();
            _employeeService = employeeService;
            _departmentService = departmentService;
        }


        private void txtUserNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnExit_Click(object sender, EventArgs e) => Application.Exit();

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var isFieldEmptyOrFilled = GetFieldsEmptyOrFilled();
            // Verifica se está vazio
            if (isFieldEmptyOrFilled)
            {
                Information.FieldIsEmptyMessage(lblUserNumber, lblPassword);
            }
            else
            {
                //Se estiver preenchido mas o usuário não existe ele exibe um erro
                //List<EMPLOYEE> employeelist = EmployeeBLL.GetEmployees(Convert.ToInt32(txtUserNo.Text), txtPassword.Text);
                var userNoConverted = Convert.ToInt32(txtUserNo.Text);
                var employeeService = _employeeService.GetEntityByIdService(userNoConverted);
                if (employeeService.ID == 0)
                    MessageBox.Show("Please control your information");
                else
                {
                    //Se existir ele pega o primeiro usuário e o mantém na sessão ativa e abre o menu
                    EMPLOYEE employee = new EMPLOYEE();
                    employee = employeeService;
                    UserStatic.EmployeeID = employee.ID;
                    UserStatic.UserNo = employee.UserNo;
                    UserStatic.isAdmin = Convert.ToBoolean(employee.isAdmin);
                    FrmMain frm = new FrmMain(_departmentService);
                    this.Hide();
                    frm.ShowDialog();
                }
            }
        }

        private bool GetFieldsEmptyOrFilled()
        {
            var userNoCondition = string.IsNullOrEmpty(txtUserNo.Text.Trim());
            var passwordCondition = string.IsNullOrEmpty(txtPassword.Text.Trim());

            var userNumberOrPasswordIsEmpty = FieldValidate(userNoCondition || passwordCondition);
            return userNumberOrPasswordIsEmpty;
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "")
                    MessageBox.Show("Please fill the User Numb and Password");
                else
                {
                    var employeelist = _employeeService.GetEmployeesByUserNoAndPasswordService(Convert.ToInt32(txtUserNo.Text), txtPassword.Text);
                    if (employeelist.Count == 0)
                        MessageBox.Show("Please control your information");
                    else
                    {
                        EMPLOYEE employee = new EMPLOYEE();
                        employee = employeelist.First();
                        UserStatic.EmployeeID = employee.ID;
                        UserStatic.UserNo = employee.UserNo;
                        UserStatic.isAdmin = Convert.ToBoolean(employee.isAdmin);
                        FrmMain frm = new FrmMain(_departmentService);
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

        public bool FieldValidate(bool condition)
        {
            var isValid = condition;

            if (isValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            //btnLogin.Enabled = false;
        }
    }
}
