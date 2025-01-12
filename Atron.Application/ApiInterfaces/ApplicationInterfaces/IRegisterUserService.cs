using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    public interface IRegisterUserService
    {
        Task<RegisterDTO> RegisterUser(RegisterDTO register);
    }
}