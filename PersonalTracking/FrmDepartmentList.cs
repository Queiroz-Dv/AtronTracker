using BLL.Interfaces;
using HLP.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PersonalTracking
{
    public partial class FrmDepartmentList : Form
    {
        private List<DepartmentModel> departmentsModelsList; // Lista de modelos de departamento
        private readonly DepartmentModel departmentModel; // Modelo do departamento atualmente selecionado
        private readonly IDepartmentService departmentService;  // Serviço responsável pelas operações relacionadas a departamento
        private readonly IEntityMessages _information; // Interface para exibição de mensagens

        public FrmDepartmentList(IDepartmentService service, IEntityMessages entityMessages)
        {
            InitializeComponent();
            _information = entityMessages; // Inicializa o objeto de mensagens com a implementação fornecida da interface IEntityMessages
            departmentModel = new DepartmentModel(_information); // Inicializa o modelo de departamento
            departmentsModelsList = new List<DepartmentModel>(); // Inicializa a lista de modelos de departamento
            departmentService = service; // Injeta o serviço de departamento na classe
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close(); // Fecha o form
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            FrmDepartment frm = new FrmDepartment(departmentService); // Cria uma instância do formulário de departamento
            this.Hide(); // Oculta o formulário atual
            frm.ShowDialog(); // Exibe o formulário de departamento para criar um novo departamento
            this.Visible = true; // Torna o formulário atual visível novamente
            FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (departmentModel.DepartmentModelId == 0)
            {
                _information.InvalidItemSelectedMessage(); // Exibe uma mensagem de erro se nenhum departamento estiver selecionado
            }
            else
            {
                FrmDepartment frm = new FrmDepartment(departmentService); // Cria uma instância do formulário de departamento
                frm.isUpdate = true; // Define a propriedade isUpdate como true para indicar que é uma atualização
                frm.department = departmentModel; // Define o departamento a ser atualizado
                Hide(); // Oculta o formulário atual
                frm.ShowDialog(); // Exibe o formulário de departamento para atualizar o departamento
                Visible = true; // Torna o formulário atual visível novamente
                FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
            }
        }

        private void FrmDepartmentList_Load(object sender, EventArgs e)
        {
            //Preenche o grid com a lista de departamentos e configura as colunas
            departmentsModelsList = departmentService.GetAllModelService().ToList();
            dgvDepartment.DataSource = departmentsModelsList.OrderBy(d => d.DepartmentModelName).ToList();
            ConfigureColumns();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = (DialogResult)_information.DeleteEntityQuestionMessage(departmentModel.DepartmentModelName); // Exibe uma mensagem de confirmação para excluir o departamento
            if (DialogResult.Yes == result)
            {
                departmentService.RemoveEntityService(departmentModel); // Remove o departamento através do serviço
            }

            FrmDepartmentList_Load(sender, e); // Recarrega a lista de departamentos
        }
    }
}