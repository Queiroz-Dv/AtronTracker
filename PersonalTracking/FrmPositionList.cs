using BLL.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmPositionList : Form
    {
        public FrmPositionList(IPositionService positionService, IEntityMessages messages, IDepartmentService departmentService)
        {
            InitializeComponent();
            _positionService = positionService;
            _departmentService = departmentService;
            _entityMessages = messages;
            positionModels = new List<PositionModel>();
            detail = new PositionModel();
        }

        private void BtnClose_Click(object sender, EventArgs e) => this.Close();

        private void BtnNew_Click(object sender, EventArgs e)
        {
            FrmPosition frm = new FrmPosition(_positionService, _entityMessages, _departmentService);
            Hide();
            frm.ShowDialog();
            Visible = true;
            FillGrid();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            //if (detail.ID.Equals(decimal.Zero))
            //    MessageBox.Show("Please select a position from table");
            //else
            //{
            //    FrmPosition frm = new FrmPosition(_positionService, _entityMessages);
            //    frm.isUpdate = true;
            //    frm.detail = detail;
            //    this.Hide();
            //    frm.ShowDialog();
            //    this.Visible = true;
            //    FillGrid();
            //}
        }

        void FillGrid()
        {
            positionModels = _positionService.GetAllService().OrderBy(pos => pos.PositionId).ToList();

            var entities = positionModels.Select(dpt => new PositionAndDepartmentInfo
            {
                PositionName = dpt.PositionName,
                DepartmentName = dpt.Department.DepartmentModelName
            }).ToList();

            dgvPositionList.DataSource = entities;
        }



        private void FrmPositionList_Load(object sender, EventArgs e)
        {
            FillGrid();

            var isPositionNameEmptyOrNull = dgvPositionList.Columns["PositionName"].Equals(null);
            var isDepartmentNameEmptyOrNull = dgvPositionList.Columns["DepartmentName"].Equals(null);

            if (!FieldValidate(isPositionNameEmptyOrNull))
            {
                var positionName = "Positions";
                dgvPositionList.Columns["PositionName"].HeaderText = positionName;
            }
            else
            {
                dgvPositionList.Columns.Add("PositionName", "Positions");
            }

            if (!FieldValidate(isDepartmentNameEmptyOrNull))
            {
                var departmentName = "Departments";
                dgvPositionList.Columns["DepartmentName"].HeaderText = departmentName;
            }
            else
            {
                dgvPositionList.Columns.Add("DepartmentName", "Departments");
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvPositionList.Rows.Count)
            {
                if (detail.Department == null)
                {
                    detail.Department = new DepartmentModel();
                }

                detail.PositionName = dgvPositionList.Rows[e.RowIndex].Cells["PositionName"].Value.ToString();
                // detail.PositionId = ... (caso necessário)
                // detail.Department.DepartmentModelId = ... (caso necessário)
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to delete this Position", "Warning", MessageBoxButtons.YesNo);
            if (DialogResult.Yes == result)
            {
                //PositionBLL.DeletePosition(detail.ID);
                MessageBox.Show("Position was Deleted");
                FillGrid();
            }
        }
    }

    public class PositionAndDepartmentInfo
    {
        public string PositionName { get; set; }

        public string DepartmentName { get; set; }
    }
}
