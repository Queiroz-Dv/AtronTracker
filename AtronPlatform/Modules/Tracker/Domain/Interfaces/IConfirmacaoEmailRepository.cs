using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IConfirmacaoEmailRepository : IRepository<ConfirmacaoEmail>
    {
        Task<bool> GravarOuSubstituirAsync(ConfirmacaoEmail confirmacaoEmail);
        Task<ConfirmacaoEmail> ObterAtivaPorUsuarioAsync(string usuarioCodigo);
        Task RegistrarTentativaFalhaAsync(int id);
        Task<bool> MarcarConfirmadaAsync(int id);
    }
}
