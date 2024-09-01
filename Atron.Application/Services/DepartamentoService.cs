using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using AutoMapper;
using Notification.Models;
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

        public List<NotificationMessage> notificationMessages { get; set; }

        public List<Message> Messages { get; set; }

        public DepartamentoService(IMapper mapper, IDepartamentoRepository departamentoRepository,
            NotificationModel<Departamento> notification, MessageModel<Departamento> messageModel)
        {
            /* A Injeção de Dependência via construtor é usada para fornecer 
             * a dependência ao DepartamentoService. 
             * Isso é feito passando apenas a abstração (IDepartamentoRepository) 
             * sem instanciar a classe concreta dentro do serviço.
             * Isso permite que o DepartamentoService não precise saber
             * como o repositório executa suas funções, promovendo o desacoplamento e a testabilidade. */
            _mapper = mapper;
            _departamentoRepository = departamentoRepository;
            _notification = notification;
            this.messageModel = messageModel;
            notificationMessages = new List<NotificationMessage>();
            Messages = new List<Message>();
        }

        public async Task AtualizarAsync(DepartamentoDTO departamentoDTO)
        {
            // Sempre que preciso conectar no repository
            // também terei de remapear o objeto para um dto 
            var departamento = _mapper.Map<Departamento>(departamentoDTO);
            await _departamentoRepository.AtualizarDepartamentoRepositoryAsync(departamento);
            Messages.Add(new Message() { Description = $"Departamento: {departamento.Codigo} foi atualizado com sucesso.", Level = Shared.Enums.MessageLevel.Message });
            //notificationMessages.Add(new NotificationMessage($"Departamento: {departamento.Codigo} foi atualizado com sucesso."));
        }

        public async Task CriarAsync(DepartamentoDTO departamentoDTO)
        {
            departamentoDTO.IdSequencial = departamentoDTO.NovoSequencial();

            var departamento = _mapper.Map<Departamento>(departamentoDTO);
            messageModel.Validate(departamento);
            if (!messageModel.Messages.HasErrors())
            {
                await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
                Messages.Add(new Message() {Description = "Departamento criado com sucesso", Level=  Shared.Enums.MessageLevel.Message});
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is not null)
            {
                var departamentoDTO = _mapper.Map<DepartamentoDTO>(departamento);
                return departamentoDTO;
            }
            else
            {
                var message = new NotificationMessage("Código do departamento informado não existe.", Notification.Enums.ENotificationType.Error);
                notificationMessages.Add(message);
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
            await _departamentoRepository.RemoverDepartmentoRepositoryAsync(departamento);
            var message = new NotificationMessage("Departamento removido com sucesso.");
            notificationMessages.Add(message);
        }
    }
}
