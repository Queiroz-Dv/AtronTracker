using Atron.Domain.Entities;
using Notification.Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.ViewsInterfaces
{
    public interface IDepartamentoViewRepository : INotificationDTO
    {
        Task<List<Dictionary<string, object>>> GetDepartamentosAsync();

        Task CriarDepartamento(Departamento departamento);

        public string ResponseResultApiJson { get; }
    }
}