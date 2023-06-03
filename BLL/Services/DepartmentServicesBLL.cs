using DAL.DTO;
using NTF.NotificationCommand;
using NTF.Services;

namespace BLL.Services
{
    public class DepartmentServicesBLL : NotificationService
    {
        private readonly DepartmentDTO _entity;

        public DepartmentServicesBLL(int departmentId, string departmentName)
        {
            // Cria a entidade para o service
            _entity = new DepartmentDTO(departmentId, departmentName);

            //Chama notification para adicionar as mensagens
            NotificationEntity = _entity;
        }

        public void SaveDepartmentService()
        {
            // Cria o command para a entidade
            var command = new SaveCommandEntity(_entity);

            // Roda o command
            command.RunCommand();
        }
    }
}
