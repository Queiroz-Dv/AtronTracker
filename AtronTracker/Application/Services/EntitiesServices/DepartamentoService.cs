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

        public async Task<Resultado> AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any()) return Resultado.Falha(erros);

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            await _asyncMap.MapToEntityAsync(departamentoDTO, entidade);

            var atualizado = await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(entidade);
            if (!atualizado)
            {
                return Resultado.Falha(string.Format(DepartamentoResource.ErroInesperadoAtualizacao, codigo));
            }

            var notificacoes = new NotificationBag();
            notificacoes.AdicionarMensagem(string.Format(DepartamentoResource.MensagemAtualizacao, codigo));
            return Resultado.Sucesso(entidade, [.. notificacoes.Messages]);
        }

        public async Task<Resultado> CriarAsync(DepartamentoDTO departamentoDTO)
        {
            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
            {
                return Resultado.Falha(erros);
            }

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);
            if (departamentoExiste != null)
            {
                return Resultado.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);
            }

            var departamento = await _asyncMap.MapToEntityAsync(departamentoDTO);

            var foiCriado = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!foiCriado)
            {
                return Resultado.Falha(DepartamentoResource.ErroGravacao);
            }

            var notificacoes = new NotificationBag();
            notificacoes.MensagemRegistroSalvo(departamento.Codigo);
            return Resultado.Sucesso(departamento, [.. notificacoes.Messages]);

        }

        public async Task<Resultado> ObterPorCodigo(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (entidade != null)
            {
                var dto = await _asyncMap.MapToDTOAsync(entidade);
                return Resultado.Sucesso(dto);
            }

            return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado> ObterPorIdAsync(int? departamentoId)
        {
            if (departamentoId == 0)
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _departamentoRepository.ObterDepartamentoPorIdRepositoryAsync(departamentoId);

            if (entidade != null)
            {
                var dto = await _asyncMap.MapToDTOAsync(entidade);
                return Resultado.Sucesso(dto);
            }

            return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }

        public async Task<Resultado> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado.Sucesso(dtos);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento != null)
            {
                var relacionamentos = await _relacionamentoRepository.ObterPorDepartamento(departamento.Id, departamento.Codigo);
                var cargos = await _cargoRepository.ObterCargosPorDepartamento(departamento.Id, departamento.Codigo);

                if (relacionamentos.Any() || cargos.Any())
                {
                    return Resultado.Falha(string.Format(DepartamentoResource.ErroDepartamentoContemRelacionamento, codigo));
                }

                var removido = await _departamentoRepository.RemoverDepartmentoRepositoryAsync(departamento);

                if (!removido)
                {
                    return Resultado.Falha(string.Format(DepartamentoResource.ErroRemocao, codigo));
                }

                var notificacoes = new NotificationBag();
                notificacoes.AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
                return Resultado.Sucesso(departamento, [.. notificacoes.Messages]);
            }

            return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        }
    }
}