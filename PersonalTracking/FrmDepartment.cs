using BLL.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : Form
    {
        public FrmDepartment(IDepartmentService departmentService, IEntityMessages message)
        {
            InitializeComponent();
            _information = message;
            department = new DepartmentModel();
            _departmentService = departmentService;
        }

        private void BtnClose_Click(object sender, EventArgs e) => this.Close();

        private void BtnSave_Click(object sender, EventArgs e)
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

        private void UpdateDepartment(DepartmentModel department)
        {
            _departmentService.UpdateEntityService(department);
            _information.EntityUpdatedMessage(department.DepartmentModelName);
            ClearFields();
        }

        private void SaveDepartment(DepartmentModel department)
        {
            _departmentService.CreateEntityService(department);
            if (_departmentService.Messages.Count > decimal.Zero)
            {
                var messagebox = new List<DialogResult>();

                foreach (var item in _departmentService.Messages)
                {
                    var message = MessageBox.Show(item.TypeMessage, item.Message, MessageBoxButtons.OK);
                    messagebox.Add(message);
                }
            }
            
            _information.EntitySavedWithSuccessMessage(department.DepartmentModelName);
            ClearFields();
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
    }
}