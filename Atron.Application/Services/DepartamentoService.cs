using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Application.Specifications.Departamento;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Services
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
         *  em vez de uma implementação concreta (DepartamentoRepository).*/
        private readonly IMapper _mapper;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly NotificationModel<Departamento> _notification;
        private readonly MessageModel<Departamento> messageModel;

        public DepartamentoService(IMapper mapper, IDepartamentoRepository departamentoRepository,
             MessageModel<Departamento> messageModel)
        {
            /* A Injeção de Dependência via construtor é usada para fornecer 
             * a dependência ao DepartamentoService. 
             * Isso é feito passando apenas a abstração (IDepartamentoRepository) 
             * sem instanciar a classe concreta dentro do serviço.
             * Isso permite que o DepartamentoService não precise saber
             * como o repositório executa suas funções, promovendo o desacoplamento e a testabilidade. */
            _mapper = mapper;
            _departamentoRepository = departamentoRepository;
            this.messageModel = messageModel;
        }

        public async Task AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
        {
            if (!new DepartamentoSpecification(codigo).IsSatisfiedBy(departamentoDTO) || 
                string.IsNullOrEmpty(codigo))
            {
                messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return;
            }

            var departamento = _mapper.Map<Departamento>(departamentoDTO);
            messageModel.Validate(departamento);

            if (!messageModel.Messages.HasErrors())
            {
                await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(departamento);
                messageModel.AddUpdateMessage(nameof(Departamento));
            }
        }

        public async Task CriarAsync(DepartamentoDTO departamentoDTO)
        {
            if (departamentoDTO is null)
            {
                messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                return;
            }

            var departamento = _mapper.Map<Departamento>(departamentoDTO);

            var entity = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamento.Codigo);

            if (entity is not null)
            {
                messageModel.AddRegisterExistMessage(nameof(Departamento));
            }

            messageModel.Validate(departamento);
            if (!messageModel.Messages.HasErrors())
            {
                await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
                messageModel.AddSuccessMessage(nameof(Departamento));
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is not null)
            {
                return _mapper.Map<DepartamentoDTO>(departamento);
            }
            else
            {
                messageModel.AddRegisterNotFoundMessage(nameof(Departamento));
                return null;
            }
        }

        public async Task<DepartamentoDTO> ObterPorIdAsync(int? departamentoId)
        {
            var entity = await _departamentoRepository.ObterDepartamentoPorIdRepositoryAsync(departamentoId);
            var dto = _mapper.Map<DepartamentoDTO>(entity);
            return dto;
        }

        public async Task<IEnumerable<DepartamentoDTO>> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            var dtos = _mapper.Map<List<DepartamentoDTO>>(entities);
            return dtos;
        }

        public async Task RemoverAsync(string codigo)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is not null)
            {
                await _departamentoRepository.RemoverDepartmentoRepositoryAsync(departamento);
                messageModel.AddRegisterRemovedSuccessMessage(nameof(Departamento));
            }
            else
            {
                messageModel.AddRegisterNotFoundMessage(nameof(Departamento));
            }
        }

        public IList<Message> GetMessages()
        {
            return messageModel.Messages;
        }
    }
}