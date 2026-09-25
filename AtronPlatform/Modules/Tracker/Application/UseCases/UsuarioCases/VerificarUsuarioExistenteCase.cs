using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Domain.ValueObjects;
using Shared.Application.Resources;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class VerificarUsuarioExistenteCase(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        public async Task<Resultado> ExecutarAsync(string codigo, string email)
        {
            var codigoUsuario = codigo.ToUpper();

            // Verifica no identity primeiro para confirmar que ainda existe 
            var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(codigoUsuario, email);
            if (contaExiste)
                return Resultado.Falha().ComMensagemRegistroExistente(codigo);

            var usuarioExistente = await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(codigoUsuario);
            if (usuarioExistente != null)
                return Resultado.Falha().ComMensagemRegistroExistente(codigo);

            if (!email.IsNullOrEmpty())
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(email);
                if (emailExiste)
                    return Resultado.Falha(EmailResource.ErroEmailUtilizado);
            }

            return Resultado.Sucesso();
        }
    }
}