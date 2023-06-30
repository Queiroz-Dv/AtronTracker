using BLL.Interfaces;
using HLP.Entity;
using HLP.Interfaces;
using MaterialSkin.Controls;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : MaterialForm
    {
        private readonly IDepartmentService departmentService;
        private readonly DepartmentModel departmentModel;
        private readonly IEntityMessages _information;
        private List<DepartmentModel> departmentsModelsList;

        public FrmDepartmentList(IDepartmentService service)
        {
            InitializeComponent();
            _information = new InformationMessage();
            departmentModel = new DepartmentModel(_information);
            departmentsModelsList = new List<DepartmentModel>();
            departmentService = service;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmDepartment frm = new FrmDepartment(departmentService);
            this.Hide();
            frm.ShowDialog();
            this.Visible = true;
            FrmDepartmentList_Load(sender, e);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (departmentModel.ID == 0)
            {
                _information.InvalidItemSelectedMessage();
            }
            else
            {
                FrmDepartment frm = new FrmDepartment(departmentService);
                frm.isUpdate = true;
                frm.department = departmentModel;
                Hide();
                frm.ShowDialog();
                Visible = true;
                FrmDepartmentList_Load(sender, e);
            }
        }

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            departmentsModelsList = departmentService.GetAllModelService().ToList();
            dgvDepartment.DataSource = departmentsModelsList.OrderBy(d => d.DepartmentName).ToList();
            ConfigureColumns();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = (DialogResult)_information.DeleteEntityQuestionMessage(departmentModel.DepartmentName);
            if (DialogResult.Yes == result)
            {
                departmentService.RemoveEntityService(departmentModel);
            }

            FrmDepartmentList_Load(sender, e);
        }
    }
}