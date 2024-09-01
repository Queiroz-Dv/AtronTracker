using Atron.Application.DTO;
using Atron.Domain.Entities;
using AutoMapper;

namespace Atron.Application.Mapping
{
    /// <summary>
    /// Classe de mapeamento dos DTOs e entidades
    /// </summary>
    public class DomainToDtoMappingProfile : Profile
    {
        public DomainToDtoMappingProfile()
        {
            CriarMapeamentoDeDepartamento();
            CriarMapeamentoDeCargo();
            CriarMapeamentoDeUsuario();
            CriarMapeamentoDeTarefa();
            CriarMapeamentoDeSalario();
            CriarMapeamentoDeMes();
            CriarMapeamentoDePermissao();
        }

        private void CriarMapeamentoDePermissao()
        {
            CreateMap<Permissao, PermissaoDTO>()
                            .ForPath(dto => dto.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
                            .ForPath(dto => dto.UsuarioCodigo, opt => opt.MapFrom(src => src.UsuarioCodigo))
                            .ForPath(dto => dto.Usuario.Id, opt => opt.MapFrom(src => src.UsuarioId))
                            .ForPath(dto => dto.Usuario.Codigo, opt => opt.MapFrom(src => src.UsuarioCodigo))
                            .ReverseMap();
        }

        private void CriarMapeamentoDeMes()
        {
            CreateMap<Mes, MesDTO>()
                .ForPath(dto => dto.Id, opt => opt.MapFrom(src => src.MesId))
                .ForPath(dto => dto.Descricao, opt => opt.MapFrom(src => src.Descricao)).ReverseMap();
        }

        private void CriarMapeamentoDeSalario()
        {
            CreateMap<Salario, SalarioDTO>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForPath(dto => dto.SalarioMensal, opt => opt.MapFrom(src => src.SalarioMensal))
                .ForPath(dto => dto.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
                .ForPath(dto => dto.Usuario.Id, opt => opt.MapFrom(src => src.UsuarioId))
                .ForPath(dto => dto.MesId, opt => opt.MapFrom(src => src.MesId))
                .ForPath(dto => dto.Mes.Id, opt => opt.MapFrom(src => src.MesId))
                .ForPath(dto => dto.Ano, opt => opt.MapFrom(src => src.Ano))
                .ReverseMap();
        }

        private void CriarMapeamentoDeTarefa()
        {
            CreateMap<Tarefa, TarefaDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeUsuario()
        {
            CreateMap<Usuario, UsuarioDTO>()
                                .ForPath(dto => dto.DepartamentoId, opt => opt.MapFrom(src => src.DepartamentoId))
                                .ForPath(dto => dto.DepartamentoCodigo, opt => opt.MapFrom(src => src.DepartamentoCodigo))
                                .ForPath(dto => dto.CargoId, opt => opt.MapFrom(src => src.CargoId))
                                .ForPath(dto => dto.CargoCodigo, opt => opt.MapFrom(src => src.CargoCodigo))
                                .ReverseMap();
        }

        private void CriarMapeamentoDeCargo()
        {
            CreateMap<Cargo, CargoDTO>().ForMember(dto => dto.DepartamentoCodigo, opt => opt.MapFrom(src => src.DepartmentoCodigo)).ReverseMap();
        }

        private void CriarMapeamentoDeDepartamento()
        {
            CreateMap<Departamento, DepartamentoDTO>().ReverseMap();
        }
    }
}