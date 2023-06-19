using BLL;
using DAL;
using DAL.DTO;
using HLP.Entity;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : MaterialForm
    {
        IEnumerable<DEPARTMENT> departments = new List<DEPARTMENT>();
        private DepartmentDTO departmentDTO = new DepartmentDTO();
        private InformationMessage Information = new InformationMessage();

        const bool condition = true;

        public FrmDepartmentList()
        {
            InitializeComponent();
            ConfigureCollorPallet();
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
                FrmDepartment frm = new FrmDepartment();
                frm.isUpdate = true;
                frm.department = departmentDTO;
                Hide();
                frm.ShowDialog();
                Visible = true;
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
            dgvDepartment.DataSource = entities.OrderBy(d => d.DepartmentName).ToList();
            dgvDepartment.Columns[0].Width = 10;
            dgvDepartment.Columns[0].Visible = false;
            dgvDepartment.Columns[1].HeaderText = "Department Name";

        }

        private void dgvDeparments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            departmentDTO.ID = Convert.ToInt32(dgvDepartment.Rows[e.RowIndex].Cells[0].Value);
            departmentDTO.DepartmentName = dgvDepartment.Rows[e.RowIndex].Cells[1].Value.ToString();
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
