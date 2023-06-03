using BLL;
using BLL.Services;
using DAL;
using MDL;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : Form
    {
        public bool isUpdate = false;
        public DEPARTMENT department;

        public FrmDepartment()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtDepartment.Text.Trim()== "")
            {
                MessageBox.Show("Please fill the name field");
            }
            else
            {
                if(!isUpdate)
                {
                    department.DepartmentName = txtDepartment.Text;
                    var departmentService = new DepartmentServicesBLL(department.ID, department.DepartmentName);

                    SaveDepartment(departmentService, department);
                    
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure?", "Warning!!", MessageBoxButtons.YesNo);
                    if (DialogResult.Yes == result)
                    {
                        DepartamentModel departmentModel = new DepartamentModel(department.ID, txtDepartment.Text);
                        DepartmentBLL.UpdateDepartment(departmentModel);
                        MessageBox.Show("Department was updated");
                        this.Close();
                    }
                }
            }
        }

        private void SaveDepartment(DepartmentServicesBLL departmentService, DEPARTMENT department)
        {
            DeparmentService
            DepartmentBLL.AddDepartment(department);
            MessageBox.Show("Department was added");
            txtDepartment.Clear();
        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
                txtDepartment.Text = department.DepartmentName;
        }
    }
}
