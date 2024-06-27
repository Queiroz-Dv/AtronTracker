using Atron.Application.DTO;
using Atron.Domain.Entities;
using AutoMapper;

namespace Atron.Application.Mapping
{
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CreateMap<Departamento, DepartamentoDTO>().ReverseMap();

            CreateMap<Cargo, CargoDTO>().ForMember(dto => dto.DepartamentoCodigo, opt => opt.MapFrom(src => src.DepartmentoCodigo)).ReverseMap();

            CreateMap<Usuario, UsuarioDTO>()
                    .ForPath(dto => dto.DepartamentoId, opt => opt.MapFrom(src => src.DepartamentoId))
                    .ForPath(dto => dto.CargoId, opt => opt.MapFrom(src => src.CargoId))
                    .ForPath(dto => dto.CargoCodigo, opt => opt.MapFrom(src => src.CargoCodigo))
                    .ForPath(dto => dto.DepartamentoCodigo, opt => opt.MapFrom(src => src.DepartamentoCodigo))
                    .ReverseMap();
        }
    }
}