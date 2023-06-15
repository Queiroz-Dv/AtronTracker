using BLL;
using DAL;
using HLP.Entity;
using HLP.Helpers;
using HLP.Interfaces;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace PersonalTracking
{
    public partial class FrmRegister : MaterialForm, IValidateHelper
    {
        private readonly IEntityMessages Information;
        private readonly EmployeeBLL employeeBLL = new EmployeeBLL();

        public FrmRegister()
        {
            InitializeComponent();
            ConfigureCollorPallet();
        }

        public void ConfigureCollorPallet()
        {
            grpBasicInfo.BackColor = Color.Gainsboro;
            grpUserInformation.BackColor = Color.Gainsboro;
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(
               Primary.DeepPurple900, Primary.DeepPurple500,
               Primary.Purple500, Accent.Purple200,
               TextShade.WHITE);
        }

        private void txtUserNo_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = General.isNumber(e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            EMPLOYEE employee = new EMPLOYEE
            {
                Name = txtName.Text,
                Surname = txtSurname.Text,
                UserNo = Convert.ToInt32(txtUserNo.Text),
                Password = txtPassword.Text
            };

            employeeBLL.CreateEntityBLL(employee);
            Information.EntitySavedWithSuccess(employee.Name);
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            txtPassword.Clear();
        }


        private void btnCheck_Click(object sender, EventArgs e)
        {
            var userNoIsEmpty = Validate(string.IsNullOrWhiteSpace(txtUserNo.Text.Trim()));

            if (userNoIsEmpty)
                ValidateErrorMessage((IEntityMessages)Information.EntitySavedWithSuccess(lblUserNumber.Text));
                Information.FieldIsEmpty(lblUserNumber.Text);
            else
            {
                bool isUniqueUser = Validate(employeeBLL.isUniqueEntity(Convert.ToInt32(txtUserNo.Text)));

                if (!isUniqueUser)
                    Information.EntityInUse(lblUserNumber.Text);
                else
                {
                    Information.EntityCanBeUse(lblUserNumber.Text);
                    InformationIsFilled(txtName.Text, txtSurname.Text, txtUserNo.Text, txtPassword.Text);
                }

            }
        }

        private void FrmRegister_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
        }

        private bool InformationIsFilled(string name, string surname, string userNo, string password)
        {
            bool nameLengthIsValid = Validate(name.Trim().Length > 3);
            bool surnameLengthIsValid = Helper.IsValid(surname.Trim().Length > 3);
            bool userNoLengthIsValid = Helper.IsValid(userNo.Trim().Length > 1);
            bool passwordLengthIsValid = Helper.IsValid(password.Length > 8);
            bool nameIsEmpty = Helper.IsValid(string.IsNullOrEmpty(txtName.Text.Trim()));
            bool surNameIsEmpty = Helper.IsValid(string.IsNullOrEmpty(txtSurname.Text.Trim()));
            bool userNoIsEmpty = Helper.IsValid(string.IsNullOrEmpty(txtUserNo.Text.Trim()));
            bool passwordIsEmpty = Helper.IsValid(string.IsNullOrEmpty(txtPassword.Text.Trim()));

            if (nameLengthIsValid && surnameLengthIsValid ||
                userNoLengthIsValid && passwordLengthIsValid)
            {
                return btnSave.Enabled = true;
            }
            else if (nameIsEmpty || surNameIsEmpty || userNoIsEmpty || passwordIsEmpty)
            {
                Information.FieldIsEmpty(lblName.Text, lblSurname.Text,
                                        lblUserNumber.Text, lblPassword.Text);
                return btnSave.Enabled = false;

            }
            else
            {
                return btnSave.Enabled = false;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool IsValid(bool condition)
        {
            return condition;
        }

        IList<string> IValidateHelper.Validate()
        {
            IList<InformationMessage> AddErros = new List<InformationMessage>();

            bool nameIsEmpty = IsValid(string.IsNullOrEmpty(txtName.Text.Trim()));
            bool surNameIsEmpty = IsValid(string.IsNullOrEmpty(txtSurname.Text.Trim()));
            bool userNoIsEmpty = IsValid(string.IsNullOrEmpty(txtUserNo.Text.Trim()));
            bool passwordIsEmpty = IsValid(string.IsNullOrEmpty(txtPassword.Text.Trim()));

            if (nameIsEmpty)
            {
                AddErros.Add(Information.FieldIsEmpty(lblName.Text));
            }

        }

        public InformationMessage ValidateErrorMessage(IEntityMessages message)
        {
            return (InformationMessage)message;
        }
    }
}
