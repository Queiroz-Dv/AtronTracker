using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class TarefaMapping : AsyncApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IAsyncApplicationMapService<UsuarioDTO, Usuario> _usuarioMap;
        private readonly IAsyncApplicationMapService<DepartamentoDTO, Departamento> _departamentoMap;
        private readonly IAsyncApplicationMapService<CargoDTO, Cargo> _cargoMap;

        public TarefaMapping(
            IAsyncApplicationMapService<UsuarioDTO, Usuario> usuarioMap,
            IAsyncApplicationMapService<DepartamentoDTO, Departamento> departamentoMap,
            IAsyncApplicationMapService<CargoDTO, Cargo> cargoMap) : base()
        {
            _usuarioMap = usuarioMap;
            _departamentoMap = departamentoMap;
            _cargoMap = cargoMap;
        }

        public override async Task<TarefaDTO> MapToDTOAsync(Tarefa entity)
        {
            var dto = new TarefaDTO
            {
                Id = entity.Id,
                Identificador = entity.Identificador,
                DestinoInicial = entity.DestinoInicial,
                ExigeAprovacaoParaObter = entity.ExigeAprovacaoParaObter,
                Titulo = entity.Titulo,
                Conteudo = entity.Conteudo,
                DataInicial = entity.DataInicial,
                DataFinal = entity.DataFinal,
                UsuarioCodigo = entity.UsuarioCodigo,
                DepartamentoCodigo = entity.DepartamentoCodigo,
                CargoCodigo = entity.CargoCodigo,
                Usuario = await MapChildAsync(entity.Usuario, _usuarioMap),
                Departamento = await MapChildAsync(entity.Departamento, _departamentoMap),
                Cargo = await MapChildAsync(entity.Cargo, _cargoMap)
            };

            if (entity.TarefaEstadoId > 0)
            {
                dto.EstadoDaTarefa = new TarefaEstadoDTO
                {
                    Id = entity.TarefaEstadoId,
                    Descricao = entity.EstadoDaTarefa?.Descricao
                };
            }

            return dto;
        }

        public override Task<Tarefa> MapToEntityAsync(TarefaDTO dto)
        {
            return Task.FromResult(new Tarefa
            {
                Id = dto.Id,
                Identificador = dto.Identificador,
                DestinoInicial = dto.DestinoInicial,
                ExigeAprovacaoParaObter = dto.ExigeAprovacaoParaObter,
                UsuarioCodigo = dto.UsuarioCodigo?.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo?.ToUpper(),
                CargoCodigo = dto.CargoCodigo?.ToUpper(),
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0,
            });
        }
    }
}
