using BLL.Interfaces;
using DAL;
using PersonalTracking.Entities;
using PersonalTracking.Helper.Entity;
using PersonalTracking.Helper.Interfaces;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    /// <summary>
    /// Formulário de registro de um novo usuário
    /// </summary>
    public partial class FrmRegister : Form
    {
        /// <summary>
        /// Campo que guarda as informações de mensagens das validações e comportamentos do formulário
        /// </summary>
        private readonly IEntityMessages Information;

        /// <summary>
        /// Classe da camada de negócios que realiza as operações e validações da entidade
        /// </summary>
        private readonly IEmployeeService employeeBLL;

        public FrmRegister()
        {
            InitializeComponent();

            // Injeta as dependências no construtor, sem isso vai estourar erro
            Information = new InformationMessage();
            //employeeBLL = new EmployeeBLL();
        }



        private void txtUserNo_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Controla o campo para aceitar apenas dígitos numéricos
            e.Handled = General.isNumber(e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Cria o objeto passando os campos preenchidos
            EMPLOYEE employee = new EMPLOYEE
            {
                Name = txtName.Text,
                Surname = txtSurname.Text,
                UserNo = Convert.ToInt32(txtUserNo.Text),
                Password = txtPassword.Text
            };

            // Chama a camada de serviços para criar a entidade
            employeeBLL.CreateEntityService(employee);

            // Exibi mensagem na tela  (precisa ter um if aqui)
            Information.EntitySavedWithSuccessMessage(employee.Name);

            // Limpa os campos
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            txtPassword.Clear();
        }

        private void FrmRegister_Load(object sender, EventArgs e) => btnSave.Enabled = false;

        /// <summary>
        /// Manipulador de eventos acionado quando o botão "Check" é clicado. Executa uma série de validações nos campos de entrada e exibe mensagens apropriadas com base nas condições.
        /// </summary>
        /// <param name="sender">O objeto que acionou o evento.</param>
        /// <param name="e">Os argumentos do evento.</param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            // Obtém a informação do campo
            var userNumberIsEmpty = string.IsNullOrWhiteSpace(txtUserNo.Text);

            // Verifica se o campo está preenchido ou não
            var userNumberIsValid = FieldValidate(userNumberIsEmpty);

            if (userNumberIsValid)
            {
                // Se não estiver preenchido adiciona messagem informando
                Information.FieldIsEmptyMessage(lblUserNumber);
            }
            else
            {
                // Senão, vai no banco de dados e verifica se tem outro igual
                var getUniqueUser = employeeBLL.IsUniqueEntity(Convert.ToInt32(txtUserNo.Text));

                // Valida se a entidade obtida está de acordo
                bool uniqueUserIsValid = FieldValidate(getUniqueUser);

                // Senão estiver 
                if (!uniqueUserIsValid)
                {
                    // Adiciona mensagem que o número do usuário está em uso
                    Information.EntityInUseMessage(lblUserNumber.Text);
                }
                else
                {
                    // Senão, informa que pode ser usado
                    Information.EntityCanBeUseMessage(lblUserNumber.Text);

                    // Chamamos o método para verificar se os campos
                    // estão preenchidos corretamente para ativar o botão de Save
                    InformationIsFilled(txtName.Text, txtSurname.Text, txtUserNo.Text, txtPassword.Text);
                }
            }
        }

        /// <summary>
        /// Calcula se os campos de entrada têm o comprimento correto de acordo com as condições especificadas.
        /// </summary>
        /// <param name="name">O valor do campo de nome.</param>
        /// <param name="surname">O valor do campo de sobrenome.</param>
        /// <param name="userNo">O valor do campo do número de usuário.</param>
        /// <param name="password">O valor do campo de senha.</param>
        /// <returns>Um valor booleano indicando se todos os campos têm o comprimento correto.</returns>
        private bool GetFieldsLenght(string name, string surname, string userNo, string password)
        {
            // Campos de condição
            var nameCondition = name.Trim().Length > 3;
            var surnameCondition = surname.Trim().Length > 3;
            var userNoCondition = userNo.Trim().Length > 1;
            var passwordCondition = password.Trim().Length < 8;

            return nameCondition
                   && surnameCondition
                   && userNoCondition
                   && passwordCondition;
        }

        /// <summary>
        /// Verifica se os campos de entrada estão vazios ou preenchidos.
        /// </summary>
        /// <returns>Um valor booleano indicando se todos os campos estão vazios ou preenchidos.</returns>
        private bool GetFieldsEmptyOrFilled()
        {
            // Campos de condição
            var nameCondition = string.IsNullOrEmpty(txtName.Text);
            var surnameCondition = string.IsNullOrEmpty(txtSurname.Text);
            var userNoCondition = string.IsNullOrEmpty(txtUserNo.Text.Trim());
            var passwordCondition = string.IsNullOrEmpty(txtPassword.Text.Trim());

            return nameCondition
                   && surnameCondition
                   && userNoCondition
                   && passwordCondition;
        }

        /// <summary>
        /// Verifica se as informações dos campos de entrada estão preenchidas corretamente.<br/>
        /// Também exibe mensagens apropriadas e habilita ou desabilita um botão de
        /// salvamento com base nas validações.
        /// </summary>
        /// <param name="name">O valor do campo de nome.</param>
        /// <param name="surname">O valor do campo de sobrenome.</param>
        /// <param name="userNo">O valor do campo do número de usuário.</param>
        /// <param name="password">O valor do campo de senha.</param>
        /// <returns>Um o botão indicando que está habilitado para salvar a entidade.</returns>
        private bool InformationIsFilled(string name, string surname, string userNo, string password)
        {
            // Validamos o comprimento dos campos passados
            var fieldsLenghtValidated = FieldValidate(GetFieldsLenght(name, surname, userNo, password));

            // Validamos se todos os campos estão preenchidos
            var fieldsEmptyOrFilledValidated = FieldValidate(GetFieldsEmptyOrFilled());

            if (fieldsLenghtValidated)
            {
                // Se o tamanho dos campos está de acordo, ou seja, 
                // eles estão preenchidos corretamente, então ativa o botão
                return btnSave.Enabled = true;
            }
            else if (fieldsEmptyOrFilledValidated)
            {
                // Se não, adicionamos a mensagem de que algum campo está vazio e não ativamos o botão
                Information.FieldIsEmptyMessage(lblName, lblSurname, lblUserNumber, lblPassword);
                return btnSave.Enabled = false;
            }
            else
            {
                // Se não cair em nenhuma condição, vai cair numa condição default 
                return btnSave.Enabled = false;
            }
        }

        /// <summary>
        /// Realiza a validação de campo com base em uma condição fornecida.
        /// </summary>
        /// <param name="condition">A condição a ser validada.</param>
        /// <returns>O valor booleano da condição de validação.</returns>
        public bool FieldValidate(bool condition) => condition;

        private void btnBack_Click(object sender, EventArgs e) => this.Close();
    }
}