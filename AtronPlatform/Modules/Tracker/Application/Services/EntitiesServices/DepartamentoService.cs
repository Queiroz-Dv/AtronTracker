using Application.DTO;
using System.Collections.Generic;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class DepartamentoService(IValidador<DepartamentoDTO> validador,
                               DepartamentoMapping mapper,
                               IDepartamentoRepository departamentoRepository,
                               IUsuarioRepository usuarioRepository,
                               ICargoRepository cargoRepository,
                               EstruturaPlanejadaPolicy estruturaPlanejadaPolicy,
                               IUsuarioCargoDepartamentoRepository relacionamentoRepository)
        : IDepartamentoService
    {
        private readonly DepartamentoMapping _mapper = mapper;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository = relacionamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly EstruturaPlanejadaPolicy _estruturaPlanejadaPolicy = estruturaPlanejadaPolicy;
        private readonly IValidador<DepartamentoDTO> _validador = validador;

        public async Task<Resultado<DepartamentoDTO>> AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
                return erros as Resultado<DepartamentoDTO>;


            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade == null)
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            DepartamentoMapping.MapToEntity(departamentoDTO, entidade);

            var resultadoGestor = await VincularGestorDepartamentoAsync(entidade, departamentoDTO.GestorDepartamentoCodigo);
            if (resultadoGestor.TeveFalha)
                return Resultado<DepartamentoDTO>.Falhas(resultadoGestor.Messages);

            var atualizado = await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(entidade);
            if (!atualizado)
                return Resultado<DepartamentoDTO>.Falha(string.Format(DepartamentoResource.ErroInesperadoAtualizacao, codigo));


            return Resultado<DepartamentoDTO>.Sucesso(departamentoDTO).AdicionarMensagem(string.Format(DepartamentoResource.MensagemAtualizacao, codigo));
        }

        public async Task<Resultado<DepartamentoDTO>> CriarAsync(DepartamentoDTO departamentoDTO)
        {
            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
                return Resultado<DepartamentoDTO>.Falhas(erros);

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);
            if (departamentoExiste != null)
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);

            var departamento = _mapper.MapToEntity(departamentoDTO);

            var resultadoGestor = await VincularGestorDepartamentoAsync(departamento, departamentoDTO.GestorDepartamentoCodigo);
            if (resultadoGestor.TeveFalha)
                return Resultado<DepartamentoDTO>.Falhas(resultadoGestor.Messages);

            var foiCriado = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!foiCriado)
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroGravacao);

            return Resultado<DepartamentoDTO>.Sucesso(departamentoDTO).ComMensagemRegistroSalvo(departamento.Codigo);
        }

        public async Task<Resultado<IEnumerable<DepartamentoDTO>>> ObterDepartamentosPorGestor(string usuarioCodigo)
        {
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<IEnumerable<DepartamentoDTO>>.Sucesso([]);

            var entidades = await _departamentoRepository.ObterDepartamentosPorCodigoGestorAsync(usuarioCodigo);
            var departamentos = _mapper.MapToDtos(entidades);

            return Resultado<IEnumerable<DepartamentoDTO>>.Sucesso(departamentos);
        }

        public async Task<Resultado<DepartamentoDTO>> ObterPorCodigo(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade != null)
            {
                var dto = _mapper.MapToDto(entidade);
                return Resultado<DepartamentoDTO>.Sucesso(dto);
            }

            return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado<DepartamentoDTO>> ObterPorIdAsync(int? departamentoId)
        {
            if (departamentoId == 0)
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _departamentoRepository.ObterDepartamentoPorIdRepositoryAsync(departamentoId);

            if (entidade != null)
            {
                var dto = _mapper.MapToDto(entidade);
                return Resultado<DepartamentoDTO>.Sucesso(dto);
            }

            return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado<List<DepartamentoDTO>>> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            var dtos = _mapper.MapToDtos(entities).ToList();
            return Resultado<List<DepartamentoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var estruturaPlanejada = await _estruturaPlanejadaPolicy.ValidarRemocaoDepartamentoAsync(departamento);
            if (estruturaPlanejada.TeveFalha)
                return estruturaPlanejada;

            var relacionamentos = await _relacionamentoRepository
                .ObterPorDepartamento(departamento.Id, departamento.Codigo);

            var cargos = await _cargoRepository
                .ObterCargosPorDepartamento(departamento.Id, departamento.Codigo);

            if (relacionamentos.Any() || cargos.Any())
                return Resultado.Falha(string.Format(DepartamentoResource.ErroDepartamentoContemRelacionamento, codigo));

            var removido = await _departamentoRepository
                .RemoverDepartmentoRepositoryAsync(departamento);

            if (!removido)
                return Resultado.Falha(string.Format(DepartamentoResource.ErroRemocao, codigo));

            return Resultado
                .Sucesso(departamento)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }

        private async Task<Resultado> VincularGestorDepartamentoAsync(Departamento departamento, string gestorCodigo)
        {
            if (gestorCodigo.IsNullOrEmpty())
            {
                departamento.GestorDepartamentoId = null;
                departamento.GestorDepartamentoCodigo = null;
                return Resultado.Sucesso();
            }

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(gestorCodigo.ToUpper());
            if (gestor is null)
                return Resultado.Falha(DepartamentoResource.ErroGestorNaoEncontrado);

            departamento.GestorDepartamentoId = gestor.Id;
            departamento.GestorDepartamentoCodigo = gestor.Codigo;

            return Resultado.Sucesso();
        }
    }
}
