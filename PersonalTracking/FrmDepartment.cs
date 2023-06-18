using BLL;
using DAL.DTO;
using HLP.Entity;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : MaterialForm
    {
        public bool isUpdate = false;
        public DepartmentDTO department = new DepartmentDTO();
        private readonly DepartmentBLL departmentBLL = new DepartmentBLL();
        public InformationMessage Information = new InformationMessage();

        public FrmDepartment()
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

        private void btnClose_Click(object sender, EventArgs e) => this.Close();


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDepartment.Text.Trim() == "")
            {
                Information.FieldIsEmptyMessage(lblDepartment.Text.ToLower());
            }
            else if (txtDepartment.Text.Trim().Length < 3)
            {
                Information.InvalidMinimumAmountCharactersMessage(lblDepartment.Text.ToLower());
            }
            else
            {
                if (!isUpdate)
                {
                    department.DepartmentName = txtDepartment.Text;
                    SaveDepartment(department);
                    Information.EntitySavedWithSuccessMessage(department.DepartmentName);
                }
                else
                {
                    // TODO: Obter a entidade do datagridview selecionado
                    DialogResult result = Information.UpdatedEntityQuestionMessage(txtDepartment.Text);
                    if (DialogResult.Yes == result)
                    {
                        department.DepartmentName = txtDepartment.Text;
                        UpdateDepartment(department);
                        Information.EntityUpdatedMessage(department.DepartmentName);
                        this.Close();
                    }
                }
            }
        }

        private void UpdateDepartment(DepartmentDTO department)
        {
            departmentBLL.UpdateEntityBLL(department);
            ClearFields();
        }

        private void SaveDepartment(DepartmentDTO department)
        {
            departmentBLL.CreateEntityBLL(department);
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
            if (txtDepartment.Text.Length >= 3)
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