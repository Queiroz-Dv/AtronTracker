using BLL;
using BLL.Interfaces;
using DAL.DTO;
using HLP.Entity;
using HLP.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmMain : Form
    {
        //171920
        private readonly IDepartmentService _departmentService;
        private readonly IPositionService _positionService;
        private readonly IEntityMessages entityMessages;

        public FrmMain(IDepartmentService departmentService, IPositionService positionService)
        {
            InitializeComponent();
            _departmentService = departmentService;
            _positionService = positionService;
            entityMessages = new InformationMessage();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEmployee_Click(object sender, EventArgs e)
        {
            if (!UserStatic.isAdmin)
            {
                EmployeeDTO dto = EmployeeBLL.GetAll();
                EmployeeDetailDTO detail = dto.Employees.First(x => x.EmployeeID == UserStatic.EmployeeID);
                FrmEmployee frm = new FrmEmployee();
                frm.detail = detail;
                frm.isUpdate = true;
                this.Hide();
                frm.ShowDialog();
                this.Visible = true;
            }
            else
            {
                FrmEmployeeList frm = new FrmEmployeeList();
                this.Hide();
                frm.ShowDialog();
                this.Visible = true;
            }
        }

        private void btnTasks_Click(object sender, EventArgs e)
        {
            FrmTaskList frm = new FrmTaskList();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void btnSalary_Click(object sender, EventArgs e)
        {
            FrmSalaryList frm = new FrmSalaryList();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void btnPermission_Click(object sender, EventArgs e)
        {
            FrmPermissionList frm = new FrmPermissionList();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void btnDeparment_Click(object sender, EventArgs e)
        {
            FrmDepartmentList frm = new FrmDepartmentList(_departmentService, entityMessages);
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void btnPosition_Click(object sender, EventArgs e)
        {
            FrmPositionList frm = new FrmPositionList(_positionService, entityMessages, _departmentService);
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            //FrmLogin frm = new FrmLogin();
            //this.Hide();
            //frm.ShowDialog();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (!UserStatic.isAdmin)
            {
                btnDeparment.Visible = false;
                btnPosition.Visible = false;
                //btnLogOut.Location=new Point(257, 252);
                //btnExit.Location=new Point(350, 282);
            }
        }
    }
}
