using Application.DTO;
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

        public async Task<Resultado> CriarAsync(CargoDTO cargoDTO)
        {
            var erros = _validador.Validar(cargoDTO);
            if (erros.Any())
            {
                return Resultado.Falha(erros);
            }

            var cargoExiste = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);
            if (cargoExiste != null)
            {
                return Resultado.Falha(CargoResource.ErroCodigoCargoExistente);
            }

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            if (departamento == null)
            {
                return Resultado.Falha(CargoResource.ErroDepartamentoNaoEncontrado);
            }

            var cargo = await _asyncMap.MapToEntityAsync(cargoDTO);
            PreencherDepartamento(departamento, cargo);

            var foiCriado = await _cargoRepository.CriarCargoAsync(cargo);
            if (!foiCriado)
            {
                return Resultado.Falha(CargoResource.ErroGravacao);
            }

            var notificacoes = new NotificationBag();
            notificacoes.MensagemRegistroSalvo(cargo.Codigo);
            return Resultado.Sucesso(cargo, [.. notificacoes.Messages]);
        }

        private static void PreencherDepartamento(Departamento departamento, Cargo cargo)
        {
            cargo.DepartamentoId = departamento.Id;
            cargo.DepartamentoCodigo = departamento.Codigo;
        }

        public async Task<Resultado> AtualizarAsync(string codigo, CargoDTO cargoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(cargoDTO);
            if (erros.Any()) return Resultado.Falha(erros);

            var entidade = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (entidade == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            if (departamento == null)
            {
                return Resultado.Falha(CargoResource.ErroDepartamentoNaoEncontrado);
            }

            await _asyncMap.MapToEntityAsync(cargoDTO, entidade);
            entidade.DepartamentoId = departamento.Id;
            entidade.DepartamentoCodigo = departamento.Codigo;

            var atualizado = await _cargoRepository.AtualizarCargoAsync(entidade);
            if (!atualizado)
            {
                return Resultado.Falha(string.Format(CargoResource.ErroInesperadoAtualizacao, codigo));
            }

            var notificacoes = new NotificationBag();
            notificacoes.AdicionarMensagem(string.Format(CargoResource.MensagemAtualizacao, codigo));
            return Resultado.Sucesso(entidade, [.. notificacoes.Messages]);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (cargo != null)
            {
                var relacionamentos = await _relacionamentoRepository.ObterPorCargo(cargo.Id, cargo.Codigo);

                if (relacionamentos.Any())
                {
                    return Resultado.Falha(string.Format(CargoResource.ErroCargoContemRelacionamento, codigo));
                }

                var removido = await _cargoRepository.RemoverCargoAsync(cargo);

                if (!removido)
                {
                    return Resultado.Falha(string.Format(CargoResource.ErroRemocao, codigo));
                }

                var notificacoes = new NotificationBag();
                notificacoes.AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
                return Resultado.Sucesso(cargo, [.. notificacoes.Messages]);
            }

            return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            if (entidade != null)
            {
                var dto = await _asyncMap.MapToDTOAsync(entidade);
                return Resultado.Sucesso(dto);
            }

            return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado> ObterTodosAsync()
        {
            var entities = await _cargoRepository.ObterCargosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado.Sucesso(dtos);
        }
    }
}