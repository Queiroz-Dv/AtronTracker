using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;
using System.Collections.Generic;

namespace Atron.Application.Mapping
{
    public class PerfilDeAcessoMapping : ApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IModuloRepository moduloRepository;

        public PerfilDeAcessoMapping(IModuloRepository moduloRepository)
        {
            this.moduloRepository = moduloRepository;
        }

        public override PerfilDeAcessoDTO MapToDTO(PerfilDeAcesso entity)
        {
            var dto = new PerfilDeAcessoDTO() { Id = entity.Id, Codigo = entity.Codigo, Descricao = entity.Descricao, };

            dto.Modulos = new List<ModuloDTO>();
            if (entity.PerfilDeAcessoModulos != null)
            {
                foreach (var item in entity.PerfilDeAcessoModulos)
                {
                    var moduloBdTask = moduloRepository.ObterPorCodigoRepository(item.ModuloCodigo);
                    moduloBdTask.Wait();
                    var moduloBd = moduloBdTask.Result;

                    var moduloDTO = new ModuloDTO()
                    {
                        Codigo = moduloBd.Codigo,
                        Descricao = moduloBd.Descricao
                    };

                    dto.Modulos.Add(moduloDTO);
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