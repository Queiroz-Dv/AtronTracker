using PersonalTracking.BLL;
using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.DTO;
using PersonalTracking.Helpers.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalTracking.DesktopUI
{
    public partial class FrmDepartmentList : Form
    {
        IEnumerable<DEPARTMENT> departments = new List<DEPARTMENT>();
        private DepartmentDTO departmentDTO = new DepartmentDTO();
        private InformationMessage Information = new InformationMessage();

        const bool condition = true;

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
            //FrmDepartment frm = new FrmDepartment();

            this.Hide();
            //frm.ShowDialog();
            this.Visible = true;
            FrmDepartmentList_Load(sender, e);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (departmentDTO.ID == 0)
            {
                Information.InvalidItemSelectedMessage();
            }
            else
            {
                //FrmDepartment frm = new FrmDepartment();
                //frm.isUpdate = true;
                //frm.department = departmentDTO;
                //Hide();
                //frm.ShowDialog();
                //Visible = true;
                FrmDepartmentList_Load(sender, e);
            }
        }

        //private void FillDgvDepartment()
        //{
        //    var departmentBLL = new DepartmentBLL();
        //    departments = departmentBLL.GetAllEntitiesBLL().OrderBy(depart => depart.DepartmentName);
        //    dgvDepartment.DataSource = departments;
        //}

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            var departmentBLL = new DepartmentBLL();
            var entities = departmentBLL.GetAllService().ToList();
            dgvDepartmentList.DataSource = entities.OrderBy(d => d.DepartmentName).ToList();
            dgvDepartmentList.Columns[0].Width = 10;
            dgvDepartmentList.Columns[0].Visible = false;
            dgvDepartmentList.Columns[1].HeaderText = "Department Name";

        }

        private void dgvDeparments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            departmentDTO.ID = Convert.ToInt32(dgvDepartmentList.Rows[e.RowIndex].Cells[0].Value);
            departmentDTO.DepartmentName = dgvDepartmentList.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = Information.DeleteEntityQuestionMessage(departmentDTO.DepartmentName);
            if (DialogResult.Yes == result)
            {
                //DepartmentBLL.DeleteDepartment(detail.ID);
                var departmentBLL = new DepartmentBLL();
                departmentBLL.RemoveEntityService(departmentDTO);
                Information.EntityDeletedWithSuccessMessage(departmentDTO.DepartmentName);
            }

            FrmDepartmentList_Load(sender, e);
        }
    }
}
