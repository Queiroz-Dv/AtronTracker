using BLL;
using DAL.DTO;
using PersonalTracking.ScreenNotifications.DepartmentNotifications;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : Form
    {
        private ICollection<DepartmentDTO> departments = new List<DepartmentDTO>();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private DepartmentDTO departmentDTO = new DepartmentDTO();

        public FrmDepartmentList()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmDepartment frm = new FrmDepartment();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            var entities = departmentBLL.GetAllEntitiesBLL();
            dgvDepartment.DataSource = entities;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (departmentDTO.ID == 0)
            {
                DeparmentInfo.InvalidDepartmentSelected();
            }
            else
            {
                FrmDepartment frm = new FrmDepartment();
                frm.isUpdate = true;
                frm.department = departmentDTO;
                Hide();
                frm.ShowDialog();
                Visible = true;
                var entities = departmentBLL.GetAllEntitiesBLL();
                dgvDepartment.DataSource = entities;
            }
        }

        //List<DEPARTMENT> list = new List<DEPARTMENT>();

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            var entities = departmentBLL.GetAllEntitiesBLL();
            dgvDepartment.DataSource = entities;
            dgvDepartment.Columns[0].Visible = false;
            dgvDepartment.Columns[1].HeaderText = "Department Name";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //DialogResult result = MessageBox.Show("Are you sure to delete this Department?", "Warning!!", MessageBoxButtons.YesNo);
            //if (DialogResult.Yes == result)
            //{
            //    DepartmentBLL.DeleteDepartment(detail.ID);
            //    MessageBox.Show("Department was Deleted");
            //    list = DepartmentBLL.GetDepartments();
            //    dataGridView1.DataSource = list;
            //}
        }


        private void dgvDeparments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            departmentDTO.ID = Convert.ToInt32(dgvDepartment.Rows[e.RowIndex].Cells[0].Value);
            departmentDTO.DepartmentName = dgvDepartment.Rows[e.RowIndex].Cells[1].Value.ToString();
        }
    }
}
