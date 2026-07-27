using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class DepartamentoMapping : AsyncApplicationMapService<DepartamentoDTO, Departamento>, IAsyncMap<DepartamentoDTO, Departamento>
    {
        public override Task<DepartamentoDTO> MapToDTOAsync(Departamento entity)
        {
            var dto = new DepartamentoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo.ToUpper(),
                Descricao = entity.Descricao.ToUpper(),
                GestorDepartamentoCodigo = entity.GestorDepartamentoCodigo,
                GestorDepartamentoNome = ObterNomeGestor(entity.GestorDepartamento)
            };

            return Task.FromResult(dto);
        }

        public override Task<Departamento> MapToEntityAsync(DepartamentoDTO dto)
        {
            var entity = new Departamento
            {
                Id = dto.Id,
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                GestorDepartamentoCodigo = dto.GestorDepartamentoCodigo?.ToUpper()
            };
            return Task.FromResult(entity);
        }

        public Task MapToEntityAsync(DepartamentoDTO dto, Departamento entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.GestorDepartamentoCodigo = dto.GestorDepartamentoCodigo?.ToUpper();
            return Task.CompletedTask;
        }

        private static string ObterNomeGestor(Usuario gestor)
        {
            if (gestor is null)
            {
                return null;
            }

            return $"{gestor.Nome} {gestor.Sobrenome}";
        }
    }
}
