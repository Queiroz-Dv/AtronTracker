using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class UsuarioMapping : Mapper<Usuario, UsuarioDTO>
    {
        private readonly IToDtoMapper<Cargo, CargoDTO> _cargoMap;
        private readonly IToDtoMapper<Departamento, DepartamentoDTO> _departamentoMap;
        private readonly IToDtoMapper<PerfilDeAcesso, PerfilDeAcessoDTO> _perfilDeAcessoMap;

        public UsuarioMapping(
            IToDtoMapper<Cargo, CargoDTO> cargoMap,
            IToDtoMapper<Departamento, DepartamentoDTO> departamentoMap,
            IToDtoMapper<PerfilDeAcesso, PerfilDeAcessoDTO> perfilDeAcessoMap)
        {
            _cargoMap = cargoMap;
            _departamentoMap = departamentoMap;
            _perfilDeAcessoMap = perfilDeAcessoMap;
        }

        public override UsuarioDTO MapToDto(Usuario entity)
        {
            var usuario = new UsuarioDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                Email = entity.Email,
                EmailConfirmado = entity.EmailConfirmado,
                DataNascimento = entity.DataNascimento,
                ReceberNotificacaoInternaTarefa = entity.ReceberNotificacaoInternaTarefa,
                ReceberNotificacaoTarefaPorEmail = entity.ReceberNotificacaoTarefaPorEmail,
                GestorImediatoCodigo = entity.GestorImediatoCodigo,
                GestorImediatoNome = ObterNomeGestor(entity.GestorImediato),
                PerfisDeAcesso = []
            };

            if (entity.UsuarioCargoDepartamentos != null)
            {
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    usuario.CargoCodigo = item.CargoCodigo;
                    usuario.Cargo = item.Cargo.MapToDto(_cargoMap);

                    usuario.DepartamentoCodigo = item.DepartamentoCodigo;
                    usuario.Departamento = item.Departamento.MapToDto(_departamentoMap);
                }
            }

            if (entity.PerfisDeAcessoUsuario != null)
            {
                foreach (var item in entity.PerfisDeAcessoUsuario)
                {
                    var perfilDeAcesso = item.PerfilDeAcesso.MapToDto(_perfilDeAcessoMap);

                    usuario.PerfisDeAcesso.Add(perfilDeAcesso);
                }
            }

            return usuario;
        }

        public override Usuario MapToEntity(UsuarioDTO dto)
        {
            return new Usuario
            {
                Codigo = dto.Codigo.ToUpper(),
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email,
                EmailConfirmado = dto.EmailConfirmado,
                DataNascimento = dto.DataNascimento,
                GestorImediatoCodigo = dto.GestorImediatoCodigo?.ToUpper(),
                ReceberNotificacaoInternaTarefa = dto.ReceberNotificacaoInternaTarefa,
                ReceberNotificacaoTarefaPorEmail = dto.ReceberNotificacaoTarefaPorEmail
            };
        }

        public void MapToEntity(UsuarioDTO dto, Usuario entityToUpdate)
        {
            entityToUpdate.Codigo = dto.Codigo?.ToUpper();
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.Sobrenome = dto.Sobrenome;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.EmailConfirmado = dto.EmailConfirmado;
            entityToUpdate.DataNascimento = dto.DataNascimento;
            entityToUpdate.GestorImediatoCodigo = dto.GestorImediatoCodigo?.ToUpper();
            entityToUpdate.ReceberNotificacaoInternaTarefa = dto.ReceberNotificacaoInternaTarefa;
            entityToUpdate.ReceberNotificacaoTarefaPorEmail = dto.ReceberNotificacaoTarefaPorEmail;

        }

        private static string ObterNomeGestor(Usuario gestor)
        {
            if (gestor is null)
            {
                return null;
            }

            return $"{gestor.Nome} {gestor.Sobrenome}".Trim();
        }
    }
}
