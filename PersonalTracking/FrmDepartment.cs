using BLL;
using DAL.DTO;
using MaterialSkin;
using MaterialSkin.Controls;
using PersonalTracking.ScreenNotifications;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartment : MaterialForm
    {
        public bool isUpdate = false;
        public DepartmentDTO department = new DepartmentDTO();
        private readonly DepartmentBLL departmentBLL = new DepartmentBLL();

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

        const bool condition = true;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDepartment.Text.Trim() == "")
            {
                InfoMessages.FieldIsEmpty(condition, lblDepartment.Text);
            }
            else if (txtDepartment.Text.Trim().Length < 3)
            {
                InfoMessages.InvalidMinimumAmountCharacters(condition, lblDepartment.Text);
            }
            else
            {
                if (!isUpdate)
                {
                    department.DepartmentName = txtDepartment.Text;
                    SaveDepartment(department);
                    InfoMessages.EntitySavedWithSuccess(department.DepartmentName);
                }
                else
                {
                    // TODO: Obter a entidade do datagridview selecionado
                    DialogResult result = InfoMessages.UpdatedEntityQuestion(condition,txtDepartment.Text);
                    if (DialogResult.Yes == result)
                    {
                        department.DepartmentName = txtDepartment.Text;
                        UpdateDepartment(department);
                        InfoMessages.EntityUpdated(department.DepartmentName);
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