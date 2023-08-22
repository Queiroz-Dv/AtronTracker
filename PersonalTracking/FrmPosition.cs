using BLL;
using BLL.Interfaces;
using DAL;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmPosition : Form
    {
        private readonly IPositionService _positionService;
        private readonly IDepartmentService _departmentService;
        private readonly IEntityMessages _entityMessages;

        public FrmPosition(IPositionService positionService, IEntityMessages entityMessages, IDepartmentService departmentService)
        {
            InitializeComponent();
            _positionService = positionService;
            _departmentService = departmentService;
            _entityMessages = entityMessages;
            _departmentList = new List<DepartmentModel>();
            _detail = new PositionModel();
        }

        private void BtnClose_Click(object sender, EventArgs e) => Close();

        public bool isUpdate = false;

        void FillCmbDepartment()
        {
            _departmentList = _departmentService.GetAllService().ToList();

            var departments = _departmentList.Select(dpt => new DepartmentInfo
            {
                Id = dpt.DepartmentModelId,
                DepartmentName = dpt.DepartmentModelName
            }).ToList();

            cmbDeparment.DataSource = departments;
        }

        private void FrmPosition_Load(object sender, EventArgs e)
        {
            FillCmbDepartment();
            cmbDeparment.DisplayMember = "DepartmentName";
            cmbDeparment.ValueMember = "Id";
            cmbDeparment.SelectedIndex = -1;
            if (isUpdate)
            {
                txtPosition.Text = _detail.PositionName;
                cmbDeparment.SelectedValue = _detail.Department.DepartmentModelId;
            }
        }

        private bool GetFieldEmptyOrFilled()
        {
            var departmentValidated = FieldValidate(string.IsNullOrEmpty(txtPosition.Text));
            return departmentValidated;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var positionIsEmpty = GetFieldEmptyOrFilled();

            if (positionIsEmpty)
            {
                _entityMessages.FieldIsEmptyMessage(lblPosition.Text);
            }
            else if (cmbDeparment.SelectedIndex.Equals(-1))
            {
                _entityMessages.InvalidItemSelectedMessage();
            }
            else
            {
                if (!isUpdate)
                {
                    var positionModel = new PositionModel();

                    _detail.PositionName = txtPosition.Text;
                    _detail.Department.DepartmentModelId = Convert.ToInt32(cmbDeparment.SelectedValue);

                    _positionService.CreateEntityService(_detail);
                    _entityMessages.EntitySavedWithSuccessMessage(_detail.PositionName);
                    txtPosition.Clear();
                    cmbDeparment.SelectedIndex = -1;
                }
                else
                {

                    var position = new PositionModel();

                    position.PositionId = _detail.PositionId;
                    position.PositionName = txtPosition.Text;
                    position.Department.DepartmentModelId = Convert.ToInt32(cmbDeparment.SelectedValue);

                    if (Convert.ToInt32(cmbDeparment.SelectedValue) != _detail.OldDepartmentID)
                    {
                        
                    }

                    //POSITION position = new POSITION();
                    //position.ID = _detail.PositionId;
                    //position.PositionName = txtPosition.Text;
                    //position.DepartmentID = Convert.ToInt32(cmbDeparment.SelectedValue);
                    //bool control = false;
                    //if (Convert.ToInt32(cmbDeparment.SelectedValue) != _detail.OldDepartmentID)
                    //    control = true;
                    //PositionBLL.UpdatePosition(position, control);
                    //MessageBox.Show("Position was updated");
                    //this.Close();
                }
            }

        }
    }

    public class DepartmentInfo
    {
        public int Id { get; set; }

        public string DepartmentName { get; set; }
    }
}
