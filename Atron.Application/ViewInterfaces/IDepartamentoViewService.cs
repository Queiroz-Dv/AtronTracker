using Atron.Application.DTO;
using Notification.Interfaces.DTO;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ViewInterfaces
{
    public interface IDepartamentoViewService : INotificationDTO
    {
        //public NotificationWebViewModel NotificationResponse { get; }

        Task<List<DepartamentoDTO>> ObterDepartamentos();

        Task CriarDepartamento(DepartamentoDTO departamento);
    }
}