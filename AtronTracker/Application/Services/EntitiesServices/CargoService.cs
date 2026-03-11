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
    /// <summary>
    /// Classe de serviço para cargos
    /// </summary>
    public class CargoService : ICargoService
    {
        private readonly IAsyncMap<CargoDTO, Cargo> _asyncMap;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository;
        private readonly IValidador<CargoDTO> _validador;

        public CargoService(IValidador<CargoDTO> validador,
                            IAsyncMap<CargoDTO, Cargo> asyncMap,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            IUsuarioCargoDepartamentoRepository relacionamentoRepository)
        {
            _validador = validador;
            _asyncMap = asyncMap;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _relacionamentoRepository = relacionamentoRepository;
        }

        public async Task<Resultado<CargoDTO>> CriarAsync(CargoDTO cargoDTO)
        {
            var erros = _validador.Validar(cargoDTO);
            if (erros.Any())
            {
                return Resultado<CargoDTO>.Falha(erros.FirstOrDefault());
            }

            var cargoExiste = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);
            if (cargoExiste != null)
            {
                return Resultado<CargoDTO>.Falha(CargoResource.ErroCodigoCargoExistente);
            }

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            if (departamento == null)
            {
                return Resultado<CargoDTO>.Falha(CargoResource.ErroDepartamentoNaoEncontrado);
            }

            var cargo = await _asyncMap.MapToEntityAsync(cargoDTO);
            cargo.VincularDepartamento(departamento);
            
            var foiCriado = await _cargoRepository.CriarCargoAsync(cargo);
            if (!foiCriado)
            {
                return Resultado<CargoDTO>.Falha(CargoResource.ErroGravacao);
            }

            return Resultado<CargoDTO>.Sucesso(cargoDTO).ComMensagemRegistroSalvo(cargoDTO.Codigo);
        }      

        public async Task<Resultado<CargoDTO>> AtualizarAsync(string codigo, CargoDTO cargoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(cargoDTO);
            if (erros.Any()) return Resultado<CargoDTO>.Falha(erros.FirstOrDefault());

            var entidade = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (entidade == null)
                return Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            if (departamento == null)
            {
                return Resultado<CargoDTO>.Falha(CargoResource.ErroDepartamentoNaoEncontrado);
            }

            await _asyncMap.MapToEntityAsync(cargoDTO, entidade);
            entidade.VincularDepartamento(departamento);

            var atualizado = await _cargoRepository.AtualizarCargoAsync(entidade);
            if (!atualizado)
            {
                return Resultado<CargoDTO>.Falha(string.Format(CargoResource.ErroInesperadoAtualizacao, codigo));
            }

            return Resultado<CargoDTO>.Sucesso(cargoDTO).AdicionarMensagem(string.Format(CargoResource.MensagemAtualizacao, codigo));
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (cargo == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var relacionamentos = await _relacionamentoRepository.ObterPorCargo(cargo.Id, cargo.Codigo);

            if (relacionamentos.Any())
                return Resultado.Falha(string.Format(CargoResource.ErroCargoContemRelacionamento, codigo));

            var removido = await _cargoRepository.RemoverCargoAsync(cargo);

            if (!removido)
                return Resultado.Falha(string.Format(CargoResource.ErroRemocao, codigo));

            return Resultado
                .Sucesso(cargo)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }


        public async Task<Resultado<CargoDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (entidade != null)
            {
                var dto = await _asyncMap.MapToDTOAsync(entidade);
                return Resultado<CargoDTO>.Sucesso(dto);
            }

            return Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado<List<CargoDTO>>> ObterTodosAsync()
        {
            var entities = await _cargoRepository.ObterCargosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado<List<CargoDTO>>.Sucesso(dtos);
        }
    }
}