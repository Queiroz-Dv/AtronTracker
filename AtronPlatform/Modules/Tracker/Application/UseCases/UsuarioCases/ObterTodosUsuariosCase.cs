using Application.DTO;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.UsuarioCases
{
    public class ObterTodosUsuariosCase(
        IToDtoMapper<Usuario, UsuarioDTO> mapper,
        IUsuarioRepository usuarioRepository)
    {
        private readonly IToDtoMapper<Usuario, UsuarioDTO> _mapper = mapper;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado<List<UsuarioDTO>>> ExecutarAsync()
        {
            var entities = await _usuarioRepository.ObterUsuariosAsync();
            var dtos = _mapper.MapToDtos(entities).ToList();
            return Resultado<List<UsuarioDTO>>.Sucesso(dtos);
        }
    }
}