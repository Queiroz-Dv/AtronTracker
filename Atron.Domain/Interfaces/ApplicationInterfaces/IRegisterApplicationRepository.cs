using Atron.Domain.ApiEntities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface IRegisterApplicationRepository
    {
        Task<bool> RegisterUserAccountAsync(Register register);
    }
}