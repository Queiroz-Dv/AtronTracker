#nullable enable
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    // Qual o objetivo da controller de módulos?
    // Centralizar operações repetitivas e segregar responsabilidades da controller dos módulos
    public class ApiBaseConfigurationController<Entity, Service> : ControllerBase
    {
        private readonly IAccessorService? serviceAccessor; // Acessor de serviços para obter serviços adicionais (opcional)
        protected readonly Service _service; // Serviço da entidade
        protected readonly Notifiable? _messageModel; // Modelo de notificações e validações para a entidade

        public ApiBaseConfigurationController(Service service, Notifiable? messageModel = null, IAccessorService? serviceAccessor = null)
        {
            // Injeta as dependências necessárias para os processos automatizados
            _service = service;
            this.serviceAccessor = serviceAccessor;
            _messageModel = messageModel;
        }

        protected T ObterService<T>() where T : class
        {
            // Método para obter serviços adicionais através do ServiceAccessor
            if (serviceAccessor is null)
                throw new InvalidOperationException("IAccessorService não foi injetado nesta controller. Adicione-o ao construtor para usar ObterService<T>().");
            return serviceAccessor.ObterService<T>();
        }

        // Serve para retornar um objeto dinâmico (aqui estou retornando a conversão em json)
        protected virtual IEnumerable<dynamic> ObterNotificacoes()
        {            
            if (serviceAccessor is null)
                throw new InvalidOperationException("IAccessorService não foi injetado nesta controller. Adicione-o ao construtor para usar ObterService<T>().");
            return _messageModel?.Notificacoes.ConvertMessageToJson();
        }
    }
}