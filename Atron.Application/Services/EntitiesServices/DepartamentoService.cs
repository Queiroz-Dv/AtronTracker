using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Application.Specifications.DepartamentoSpecifications;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services.EntitiesServices
{
    public class DepartamentoService : IDepartamentoService
    {
        /*  A Inversão de Controle consiste em não criar uma instância do repositório
         *  diretamente no construtor, mas utilizar um contêiner para lidar com isso. 
         *  Em outras palavras, você inverte o controle da criação e gerenciamento
         *  das dependências do serviço para um contêiner de IoC. 
         *  Em vez de o DepartamentoService controlar a criação do DepartamentoRepository, 
         *  essa responsabilidade é delegada ao contêiner de IoC. 
         *  O serviço depende de uma abstração (IDepartamentoRepository) 
         *  em vez de uma implementação concreta (DepartamentoRepository).
         */
        private readonly IApplicationMapService<DepartamentoDTO, Departamento> _map;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IValidateModel<Departamento> _validateModel;
        private readonly MessageModel messageModel;

        public DepartamentoService(IApplicationMapService<DepartamentoDTO, Departamento> map,
                                   IDepartamentoRepository departamentoRepository,
                                   IValidateModel<Departamento> validateModel,
                                   MessageModel messageModel,
                                   ICargoRepository cargoRepository,
                                   IUsuarioCargoDepartamentoRepository relacionamentoRepository)
        {
            /* A Injeção de Dependência via construtor é usada para fornecer 
             * a dependência ao DepartamentoService. 
             * Isso é feito passando apenas a abstração (IDepartamentoRepository) 
             * sem instanciar a classe concreta dentro do serviço.
             * Isso permite que o DepartamentoService não precise saber
             * como o repositório executa suas funções, promovendo o desacoplamento e a testabilidade. */
            _map = map;
            _departamentoRepository = departamentoRepository;
            this.messageModel = messageModel;
            _validateModel = validateModel;
            _cargoRepository = cargoRepository;
            _relacionamentoRepository = relacionamentoRepository;
        }

        public async Task AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (!new DepartamentoSpecification(codigo).IsSatisfiedBy(departamentoDTO))
            {
                messageModel.MensagemRegistroInvalido(codigo);
                return;
            }

            var departamento = _map.MapToEntity(departamentoDTO);
            _validateModel.Validate(departamento);

            if (!messageModel.Notificacoes.HasErrors())
            {
                await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(departamento);
                messageModel.MensagemRegistroAtualizado(departamentoDTO.Codigo);
            }
        }

        public async Task CriarAsync(DepartamentoDTO departamentoDTO)
        {
            if (departamentoDTO is null)
            {
                messageModel.MensagemRegistroInvalido();
                return;
            }

            var departamento = _map.MapToEntity(departamentoDTO);

            var entity = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamento.Codigo);

            if (entity is not null)
                messageModel.MensagemRegistroNaoExiste(departamentoDTO.Codigo);


            _validateModel.Validate(departamento);
            if (!messageModel.Notificacoes.HasErrors())
            {
                await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
                messageModel.MensagemRegistroSalvo(departamentoDTO.Codigo);
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is not null)
            {
                return _map.MapToDTO(departamento);
            }
            else
            {
                messageModel.MensagemRegistroNaoEncontrado(codigo);
                return null;
            }
        }

        public async Task<DepartamentoDTO> ObterPorIdAsync(int? departamentoId)
        {
            var entity = await _departamentoRepository.ObterDepartamentoPorIdRepositoryAsync(departamentoId);
            return _map.MapToDTO(entity);
        }

        public async Task<IEnumerable<DepartamentoDTO>> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            return _map.MapToListDTO(entities.ToList());
        }

        public async Task RemoverAsync(string codigo)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is not null)
            {
                var relacionamentos = await _relacionamentoRepository.ObterPorDepartamento(departamento.Id, departamento.Codigo);
                if (relacionamentos.Any())
                {
                    foreach (var item in relacionamentos)
                    {
                        await _relacionamentoRepository.RemoverRelacionamentoPorDepartamentoRepository(item);
                    }
                }

                var cargos = await _cargoRepository.ObterCargosPorDepartamento(departamento.Id, departamento.Codigo);

                if (cargos.Any())
                {
                    foreach (var item in cargos)
                    {
                        await _cargoRepository.RemoverCargoAsync(item);
                    }
                }

                var removido  = await _departamentoRepository.RemoverDepartmentoRepositoryAsync(departamento);

                if (!removido)
                {
                    messageModel.MensagemRegistroNaoEncontrado(codigo);
                    return;
                }

                messageModel.MensagemRegistroRemovido(codigo);
            }
            else
            {
                messageModel.MensagemRegistroNaoEncontrado(codigo);
            }
        }
    }
}