using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class TarefaMapping(
        IToDtoMapper<Usuario, UsuarioDTO> usuarioMap,
        IToDtoMapper<Departamento, DepartamentoDTO> departamentoMap,
        IToDtoMapper<Cargo, CargoDTO> cargoMap,
        IToDtoMapper<TarefaEstado, TarefaEstadoDTO> tarefaEstadoMap)
        : Mapper<Tarefa, TarefaDTO>
    {
        private readonly IToDtoMapper<Usuario, UsuarioDTO> _usuarioMap = usuarioMap;
        private readonly IToDtoMapper<Departamento, DepartamentoDTO> _departamentoMap = departamentoMap;
        private readonly IToDtoMapper<Cargo, CargoDTO> _cargoMap = cargoMap;
        private readonly IToDtoMapper<TarefaEstado, TarefaEstadoDTO> _tarefaEstadoMap = tarefaEstadoMap;

        public override TarefaDTO MapToDto(Tarefa entity)
        {
            var dto = new TarefaDTO
            {
                Id = entity.Id,
                DestinoInicial = (int)entity.DestinoInicial,
                ExigeAprovacaoParaObter = entity.ExigeAprovacaoParaObter,
                Titulo = entity.Titulo,
                Conteudo = entity.Conteudo,
                DataInicial = entity.DataInicial,
                DataFinal = entity.DataFinal,
                UsuarioCodigo = entity.UsuarioCodigo,
                DepartamentoCodigo = entity.DepartamentoCodigo,
                CargoCodigo = entity.CargoCodigo,
                Usuario = entity.Usuario.MapToDto(_usuarioMap),
                Departamento = entity.Departamento.MapToDto(_departamentoMap),
                Cargo = entity.Cargo.MapToDto(_cargoMap),
                EstadoDaTarefa = entity.EstadoDaTarefa.MapToDto(_tarefaEstadoMap)
            };

            return dto;
        }

        public override Tarefa MapToEntity(TarefaDTO dto)
        {
            return new Tarefa
            {
                Id = dto.Id,
                DestinoInicial = (Domain.Enums.DestinoInicialTarefa)dto.DestinoInicial,
                ExigeAprovacaoParaObter = dto.ExigeAprovacaoParaObter,
                UsuarioCodigo = dto.UsuarioCodigo,
                DepartamentoCodigo = dto.DepartamentoCodigo,
                CargoCodigo = dto.CargoCodigo,
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0
            };
        }
    }
}
