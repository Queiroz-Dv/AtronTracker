using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using BLL.Interfaces;
using DAL.DTO;
using HLP.Interfaces;

namespace PersonalTracking
{
    public partial class FrmPositionList : Form
    {
        public FrmPositionList(IPositionService positionService, IEntityMessages messages)
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmPosition frm = new FrmPosition();
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            FillGrid();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (detail.ID == 0)
                MessageBox.Show("Please select a position from table");
            else
            {
                FrmPosition frm = new FrmPosition();
                frm.isUpdate = true;
                frm.detail = detail;
                this.Hide();
                frm.ShowDialog();
                this.Visible = true;
                FillGrid();
            }
        }

        List<PositionDTO> positionList = new List<PositionDTO>();

        void FillGrid()
        {
            positionList = PositionBLL.GetPositions();
            dgvPositionList.DataSource = positionList;
        }
        PositionDTO detail = new PositionDTO();

        private void FrmPositionList_Load(object sender, EventArgs e)
        {
            FillGrid();
            dgvPositionList.Columns[1].Visible = false;
            dgvPositionList.Columns[4].Visible = false;
            dgvPositionList.Columns[2].Visible = false;
            dgvPositionList.Columns[0].HeaderText = "Deparment Name";
            dgvPositionList.Columns[3].HeaderText = "Position Name";
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            detail.PositionName = dgvPositionList.Rows[e.RowIndex].Cells[3].Value.ToString();
            detail.ID = Convert.ToInt32(dgvPositionList.Rows[e.RowIndex].Cells[2].Value);
            detail.DepartmentID = Convert.ToInt32(dgvPositionList.Rows[e.RowIndex].Cells[2].Value);
            detail.OldDepartmentID = Convert.ToInt32(dgvPositionList.Rows[e.RowIndex].Cells[4].Value);


        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to delete this Position", "Warning", MessageBoxButtons.YesNo);
            if(DialogResult.Yes==result)
            {
                PositionBLL.DeletePosition(detail.ID);
                MessageBox.Show("Position was Deleted");
                FillGrid();
            }
        }
    }
}
