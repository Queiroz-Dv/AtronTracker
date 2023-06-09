using BLL;
using DAL.DTO;
using PersonalTracking.ScreenNotifications.DepartmentNotifications;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : Form
    {
        public bool isUpdate = false;
        public DepartmentDTO department = new DepartmentDTO();
        public DepartmentBLL departmentBLL = new DepartmentBLL();

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
            if (txtDepartment.Text.Trim() == "")
            {
                DeparmentInfo.DeparmentFieldIsEmpty();
            }
            else if (txtDepartment.Text.Trim().Length < 3)
            {
                DeparmentInfo.InvalidMinimumAmountDepartmentCharacters();
            }
            else
            {
                if (!isUpdate)
                {
                    department.DepartmentName = txtDepartment.Text;
                    SaveDepartment(department);
                    DeparmentInfo.DeparmentSavedWithSuccess(department);
                }
                else
                {
                    DialogResult result = MessageBox.Show("Are you sure?", "Warning!!", MessageBoxButtons.YesNo);
                    if (DialogResult.Yes == result)
                    {
                        department.DepartmentName = txtDepartment.Text;
                        UpdateDepartment(department);
                        MessageBox.Show("Department was updated");
                        this.Close();
                    }
                }
            }
        }

        private void UpdateDepartment(DepartmentDTO department)
        {
            departmentBLL.UpdateEntityBLL(department);
            ClearFields();
        }

        private void SaveDepartment(DepartmentDTO department)
        {
            departmentBLL.CreateEntityBLL(department);
            ClearFields();
        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                txtDepartment.Text = department.DepartmentName;
            }
        }

        private void ClearFields()
        {
            txtDepartment.Clear();
        }
    }
}