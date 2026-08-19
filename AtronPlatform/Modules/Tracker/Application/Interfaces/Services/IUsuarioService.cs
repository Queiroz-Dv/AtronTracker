using Application.DTO;
using Application.DTO.Request;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<Resultado<List<UsuarioDTO>>> ObterTodosAsync();

        Task<Resultado<UsuarioDTO>> ObterPorCodigoAsync(string codigo);

        Task<Resultado<UsuarioRequest>> CriarAsync(UsuarioRequest request);

        Task<Resultado<UsuarioRequest>> AtualizarAsync(UsuarioRequest request);

        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado> DesativarAsync(string codigo);

        Task<Resultado> AlterarEmailAsync(string codigo, string emailNovo);

        Task<Resultado> ConfirmarAlteracaoEmailAsync(string usuarioCodigo, string emailNovo, string token);

        Task<Resultado> ReenviarConfirmacaoEmailAsync(string codigo);

        Task<Resultado<Usuario>> ObterUsuarioAtual();
    }
}
