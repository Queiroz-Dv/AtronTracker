using BLL.Interfaces;
using HLP.Entity;
using MaterialSkin.Controls;
using PersonalTracking.Models;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : MaterialForm
    {
        public FrmDepartment(IDepartmentService departmentService)
        {
            InitializeComponent();
            _information = new InformationMessage();
            department = new DepartmentModel(_information);
            _departmentService = departmentService;
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();


        private void btnSave_Click(object sender, EventArgs e)
        {
            var departmentIsEmpty = GetFieldEmptyOrFilled();
            var departmentAmountCharactersIsValid = GetFieldLength();

            if (departmentIsEmpty)
            {
                _information.FieldIsEmptyMessage(lblDepartment.Text.ToLower());
            }
            else if (departmentAmountCharactersIsValid)
            {
                _information.InvalidMinimumAmountCharactersMessage(lblDepartment.Text.ToLower());
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
                    var result = (DialogResult)_information.UpdatedEntityQuestionMessage(txtDepartment.Text);
                    if (DialogResult.Yes == result)
                    {
                        department.DepartmentName = txtDepartment.Text;
                        UpdateDepartment(department);
                        this.Close();
                    }
                }
            }
        }

        private void UpdateDepartment(DepartmentModel department)
        {
            _departmentService.UpdateEntityService(department);
            ClearFields();
        }

        private void SaveDepartment(DepartmentModel department)
        {
            _departmentService.CreateEntityService(department);
            ClearFields();
        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                txtDepartment.Text = department.DepartmentName;
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
        }

        private void ClearFields() => txtDepartment.Clear();

        private void txtDepartment_TextChanged(object sender, EventArgs e)
        {
            var departmentVerification = GetDepartmentFieldLengthAmount();

            if (departmentVerification)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
        }
    }
}