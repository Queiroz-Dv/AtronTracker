using BLL;
using BLL.Services;
using DAL.DTO;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : Form
    {
        public bool isUpdate = false;
        public DepartmentDTO department = new DepartmentDTO();

        public FrmDepartment()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDepartment.Text.Trim() == "")
            {
                MessageBox.Show("Please fill the name field");
            }
            else
            {
                if (!isUpdate)
                {
                    department.DepartmentName = txtDepartment.Text;
                    SaveDepartment(department);

                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure?", "Warning!!", MessageBoxButtons.YesNo);
                    if (DialogResult.Yes == result)
                    {
                        //DepartamentModel departmentModel = new DepartamentModel(department.ID, txtDepartment.Text);
                        //DepartmentBLL.UpdateDepartment(departmentModel);
                        //MessageBox.Show("Department was updated");
                        //this.Close();
                    }
                }
            }
        }

        private void SaveDepartment(DepartmentDTO department)
        {
            DepartmentBLL departmentBLL = new DepartmentBLL();
            departmentBLL.CreateDepartmentServices(department);
        }



        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
                txtDepartment.Text = department.DepartmentName;
        }
    }
}
