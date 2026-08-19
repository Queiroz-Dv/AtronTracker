using Application.DTO;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaUsuarioRelacionamentoService(
        IUsuarioRepository usuarioRepository,
        IToDtoMapper<Usuario, UsuarioDTO> usuarioMap)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IToDtoMapper<Usuario, UsuarioDTO> _usuarioMap = usuarioMap;

        public async Task<Resultado<Usuario>> RelacionarAsync(Tarefa tarefa, TarefaDTO tarefaDTO)
        {
            if (tarefaDTO.UsuarioCodigo.IsNullOrEmpty())
            {
                tarefa.RemoverUsuario();
                return Resultado<Usuario>.Sucesso(null);
            }

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo);
            if (usuario is null)
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
          
            tarefa.VincularUsuario(usuario.Id, usuario.Codigo);
            tarefaDTO.Usuario = _usuarioMap.MapToDto(usuario);

            return Resultado<Usuario>.Sucesso(usuario);
        }
    }
}
