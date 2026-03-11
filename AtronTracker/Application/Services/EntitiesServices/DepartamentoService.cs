using Application.DTO;
using System.Collections.Generic;
using Application.Interfaces.Services;
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
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IAsyncMap<DepartamentoDTO, Departamento> _asyncMap;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IValidador<DepartamentoDTO> _validador;

        public DepartamentoService(IValidador<DepartamentoDTO> validador,
                                   IAsyncMap<DepartamentoDTO, Departamento> asyncMap,
                                   IDepartamentoRepository departamentoRepository,
                                   ICargoRepository cargoRepository,
                                   IUsuarioCargoDepartamentoRepository relacionamentoRepository)
        {
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _relacionamentoRepository = relacionamentoRepository;
            _validador = validador;
            _asyncMap = asyncMap;
        }

        public async Task<Resultado<DepartamentoDTO>> AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any()) 
                return Resultado<DepartamentoDTO>.Falha(erros.FirstOrDefault());

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade == null)
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            await _asyncMap.MapToEntityAsync(departamentoDTO, entidade);

            var atualizado = await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(entidade);
            if (!atualizado)
            {
                return Resultado<DepartamentoDTO>.Falha(string.Format(DepartamentoResource.ErroInesperadoAtualizacao, codigo));
            }

            return Resultado<DepartamentoDTO>.Sucesso(departamentoDTO).AdicionarMensagem(string.Format(DepartamentoResource.MensagemAtualizacao, codigo));
        }

        public async Task<Resultado<DepartamentoDTO>> CriarAsync(DepartamentoDTO departamentoDTO)
        {
            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
            {
                return Resultado<DepartamentoDTO>.Falhas(erros);
            }

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);
            if (departamentoExiste != null)
            {
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);
            }

            var departamento = await _asyncMap.MapToEntityAsync(departamentoDTO);

            var foiCriado = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!foiCriado)
            {
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroGravacao);
            }

            return Resultado<DepartamentoDTO>.Sucesso(departamentoDTO).ComMensagemRegistroSalvo(departamento.Codigo);          
        }

        public async Task<Resultado<DepartamentoDTO>> ObterPorCodigo(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade != null)
            {
                var dto = await _asyncMap.MapToDTOAsync(entidade);
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
                var dto = await _asyncMap.MapToDTOAsync(entidade);
                return Resultado<DepartamentoDTO>.Sucesso(dto);
            }

            return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado<List<DepartamentoDTO>>> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado<List<DepartamentoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var relacionamentos = await _relacionamentoRepository
                .ObterPorDepartamento(departamento.Id, departamento.Codigo);

            var cargos = await _cargoRepository
                .ObterCargosPorDepartamento(departamento.Id, departamento.Codigo);

            if (relacionamentos.Any() || cargos.Any())
                return Resultado.Falha(
                    string.Format(DepartamentoResource.ErroDepartamentoContemRelacionamento, codigo)
                );

            var removido = await _departamentoRepository
                .RemoverDepartmentoRepositoryAsync(departamento);

            if (!removido)
                return Resultado.Falha(
                    string.Format(DepartamentoResource.ErroRemocao, codigo)
                );

            return Resultado
                .Sucesso(departamento)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }
    }
}