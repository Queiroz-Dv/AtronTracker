using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaUsuarioAtualService(IUsuarioRepository usuarioRepository, IUserAccessor userAccessor) : ITarefaUsuarioAtualService
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUserAccessor _userAccessor = userAccessor;

        public async Task<Resultado<Usuario>> ObterAsync()
        {
            var usuarioCodigo = _userAccessor.ObterCodigoUsuarioLogado();
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioCodigo);
            return usuario is null
                ? Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<Usuario>.Sucesso(usuario);
        }
    }
}