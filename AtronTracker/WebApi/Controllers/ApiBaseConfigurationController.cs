using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    // Qual o objetivo da controller de módulos?
    // Centralizar operações repetitivas e segregar responsabilidades da controller dos módulos
    public class ApiBaseConfigurationController<Entity, Service> : ControllerBase
    {
        private readonly IAccessorService serviceAccessor; // Acessor de serviços para obter serviços adicionais
        protected readonly Service _service; // Serviço da entidade
        protected readonly MessageModel _messageModel; // Modelo de notificações e validações para a entidade

        public ApiBaseConfigurationController(Service service, IAccessorService serviceAccessor, MessageModel messageModel)
        {
            // Injeta as dependências necessárias para os processos automatizados
            _service = service;
            this.serviceAccessor = serviceAccessor;
            _messageModel = messageModel;
        }

        protected T ObterService<T>() where T : class
        {
            // Método para obter serviços adicionais através do ServiceAccessor
            return serviceAccessor.ObterService<T>();
        }

        // Serve para retornar um objeto dinâmico (aqui estou retornando a conversão em json)
        protected virtual IEnumerable<dynamic> ObterNotificacoes()
        {
            // Passar para um método de extensão
            return _messageModel.Notificacoes.ConvertMessageToJson();
        }
    }
}