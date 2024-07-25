using Atron.Application.DTO;
using Notification.Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ViewInterfaces
{
    public interface IDepartamentoViewService : INotificationDTO, INotificationResponseDTO
    {
        Task<List<DepartamentoDTO>> ObterDepartamentos();

        Task CriarDepartamento(DepartamentoDTO departamento);
    }
}