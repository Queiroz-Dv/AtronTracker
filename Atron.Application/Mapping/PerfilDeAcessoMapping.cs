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
            return new PerfilDeAcessoDTO()
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Descricao = entity.Descricao

                // Verificar como mapear as coleções de Modulo e Usuario
            };
        }

        public override PerfilDeAcesso MapToEntity(PerfilDeAcessoDTO dto)
        {
            return new PerfilDeAcesso()
            {                
                Codigo = dto.Codigo,
                Descricao = dto.Descricao
            };

            //foreach (var moduloDTO in dto.Modulos)
            //{
            //    if (moduloDTO is not null)
            //    {
            //        perfilDeAcesso.PerfilDeAcessoModulos.Add(new PerfilDeAcessoModulo()
            //        {
            //            Modulo = new Modulo()
            //            {
            //                Codigo = moduloDTO.Codigo,
            //                Descricao = moduloDTO.Descricao
            //            }
            //        });
            //    }
            //}            
        }
    }
}