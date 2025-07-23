using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;
using System.Collections.Generic;

namespace Atron.Application.Mapping
{
    public class PerfilDeAcessoMapping : ApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        public override PerfilDeAcessoDTO MapToDTO(PerfilDeAcesso entity)
        {
            var dto = new PerfilDeAcessoDTO() { Id = entity.Id, Codigo = entity.Codigo, Descricao = entity.Descricao, };
            dto.Modulos = new List<ModuloDTO>();

            if (entity.PerfilDeAcessoModulos != null)
            {
                foreach (var item in entity.PerfilDeAcessoModulos)
                {
                    var moduloDto = new ModuloDTO
                    {
                        Codigo = item.Modulo.Codigo,
                        Descricao = item.Modulo.Descricao
                    };

                    dto.Modulos.Add(moduloDto);
                }
            }

            return dto;
        }

        public override PerfilDeAcesso MapToEntity(PerfilDeAcessoDTO dto)
        {
            return new PerfilDeAcesso()
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao
            };
        }
    }
}