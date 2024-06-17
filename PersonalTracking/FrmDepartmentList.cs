using BLL.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : Form
    {
        private readonly IDepartmentService _departmentService;  // Serviço responsável pelas operações relacionadas a departamento
        private readonly IEntityMessages _information; // Serviço pelas mensagens de notificação
        private readonly Department departmentModel = new Department();// Modelo do departamento atualmente selecionado
        private List<Department> departmentsModelsList; // Lista de modelos de departamento
        private FrmDepartment frm;

        public FrmDepartmentList(IDepartmentService service, IEntityMessages entityMessages)
        {
            InitializeComponent();
            _information = entityMessages; // Inicializa o objeto de mensagens com a implementação fornecida da interface IEntityMessages
            _departmentService = service; // Injeta o serviço de departamento na classe
            departmentsModelsList = new List<Department>(); // Inicializa a lista de modelos de departamento
        }

        private void ConfigureColumns()
        {
            dgvDepartment.Columns[0].Width = 10;
            dgvDepartment.Columns[0].Visible = false;
            dgvDepartment.Columns[1].HeaderText = "Department Name";
        }

        private void dgvDeparments_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            departmentModel.Id = Convert.ToInt32(dgvDepartment.Rows[e.RowIndex].Cells[0].Value);
            departmentModel.Name = dgvDepartment.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close(); // Fecha o form
        }

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            //Preenche o grid com a lista de departamentos e configura as colunas
            departmentsModelsList = _departmentService.GetAllService().ToList();
            dgvDepartment.DataSource = departmentsModelsList.OrderBy(d => d.Name).ToList();
            ConfigureColumns();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            FrmDepartment frm = new FrmDepartment(_departmentService, _information); // Cria uma instância do formulário de departamento
            this.Hide(); // Oculta o formulário atual
            frm.ShowDialog(); // Exibe o formulário de departamento para criar um novo departamento
            this.Visible = true; // Torna o formulário atual visível novamente
            FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {

            if (departmentModel.Id.Equals(decimal.Zero))
            {
                _information.InvalidItemSelectedMessage(); // Exibe uma mensagem de erro se nenhum departamento estiver selecionado
            }
            else
            {
                FrmDepartment frm = new FrmDepartment(_departmentService, _information); // Cria uma instância do formulário de departamento
                frm.isUpdate = true; // Define a propriedade isUpdate como true para indicar que é uma atualização
                frm.department = departmentModel; // Define o departamento a ser atualizado
                Hide(); // Oculta o formulário atual
                frm.ShowDialog(); // Exibe o formulário de departamento para atualizar o departamento
                Visible = true; // Torna o formulário atual visível novamente
                FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var result = (DialogResult)_information.DeleteEntityQuestionMessage(departmentModel.Name); // Exibe uma mensagem de confirmação para excluir o departamento
            if (DialogResult.Yes.Equals(result))
            {
                _departmentService.RemoveEntityService(departmentModel.Id); // Remove o departamento através do serviço
                _information.EntityDeletedWithSuccessMessage(departmentModel.Name);
            }
            FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
        }
    }
}