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

            //CreateMap<Usuario, EmployeeDTO>()
            //        .ForMember(dto => dto.DepartmentID, opt => opt.MapFrom(src => src.DepartmentoId))
            //        .ForMember(dto => dto.PositionID, opt => opt.MapFrom(src => src.CargoId))
            //        .ReverseMap();
        }
    }
}