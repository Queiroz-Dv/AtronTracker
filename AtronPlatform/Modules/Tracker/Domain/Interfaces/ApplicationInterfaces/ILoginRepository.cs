using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface ILoginRepository
    {
        Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha);

        Task<bool> ValidarCredenciaisAsync(string codigoUsuario, string senha);

    }
}
