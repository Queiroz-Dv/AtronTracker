using BLL.Interfaces;
using HLP.Interfaces;
using MaterialSkin.Controls;
using PersonalTracking.Models;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : MaterialForm
    {
        public FrmDepartment(IDepartmentService departmentService, IEntityMessages message)
        {
            InitializeComponent();
            _information = message;
            department = new DepartmentModel();
            _departmentService = departmentService;
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();


        private void btnSave_Click(object sender, EventArgs e)
        {
            var departmentIsEmpty = GetFieldEmptyOrFilled();
            var departmentAmountCharactersIsValid = GetFieldLength();
            department.DepartmentModelName = txtDepartment.Text;

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
                    SaveDepartment(department);
                }
                else
                {
                    var result = (DialogResult)_information.UpdatedEntityQuestionMessage(txtDepartment.Text);
                    if (DialogResult.Yes == result)
                    {
                        UpdateDepartment(department);
                        this.Close();
                    }
                }
            }
        }

        private void FrmDepartment_Load(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                txtDepartment.Text = department.DepartmentModelName;
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