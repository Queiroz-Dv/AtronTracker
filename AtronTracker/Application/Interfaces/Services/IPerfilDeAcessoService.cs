using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPerfilDeAcessoService
    {
        Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync();

        Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo);

        Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO);

        Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO);

        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario);

        Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(string codigo);

        Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioAsync(string usuarioCodigo);
    }
}
