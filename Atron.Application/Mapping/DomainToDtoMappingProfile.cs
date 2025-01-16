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
        //TODO: Refazer o mapeamento manualmente ao invés do AutoMapper
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
            CreateMap<Permissao, PermissaoDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeMes()
        {
            CreateMap<Mes, MesDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeSalario()
        {
            CreateMap<Salario, SalarioDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeTarefa()
        {
            CreateMap<Tarefa, TarefaDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeUsuario()
        {
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeCargo()
        {
            CreateMap<Cargo, CargoDTO>().ReverseMap();
        }

        private void CriarMapeamentoDeDepartamento()
        {
            CreateMap<Departamento, DepartamentoDTO>().ReverseMap();
        }
    }
}