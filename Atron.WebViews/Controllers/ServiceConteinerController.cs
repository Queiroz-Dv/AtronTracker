using Atron.WebViews.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atron.WebViews.Controllers
{
    /// <summary>
    /// Controlador abstrato que serve como um contêiner para todos os processos de fluxo.
    /// </summary>
    /// <typeparam name="DTO">O tipo do Objeto de Transferência de Dados.</typeparam>
    /// <typeparam name="Entity">O tipo da Entidade.</typeparam>
    public abstract class ServiceContainerController<DTO, Entity> : Controller, IViewConfiguration
    {
        /// <summary>
        /// Serviço para lidar com a lógica de paginação.
        /// </summary>
        protected IPaginationService<DTO> _paginationService;

        /// <summary>
        /// Modelo para lidar com mensagens e notificações.
        /// </summary>
        protected MessageModel _messageModel;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ServiceContainerController{DTO, Entity}"/>.
        /// </summary>
        /// <param name="messageModel">O modelo de mensagem a ser usado para notificações.</param>
        protected ServiceContainerController(MessageModel messageModel)
        {
            _messageModel = messageModel;
        }

        /// <summary>
        /// Configura a ação da página atual no ViewData.
        /// </summary>
        public void ConfigureCurrentPageAction()
        {
            SetViewData("ActionPage", _paginationService.GetPageInfo().PageRequestInfo.Action);
        }

        /// <summary>
        /// Configura a ação da página atual no ViewData com uma ação especificada.
        /// </summary>
        /// <param name="currentPageAction">A ação da página atual a ser definida.</param>
        public void ConfigureCurrentPageAction(string currentPageAction)
        {
            SetViewData("ActionPage", currentPageAction);
        }

        /// <summary>
        /// Configura o título para a visualização no ViewData.
        /// </summary>
        /// <param name="title">O título a ser definido.</param>
        public void ConfigureDataTitleForView(string title)
        {
            SetViewData("Title", title);
        }

        /// <summary>
        /// Configura o controlador atual no ViewBag.
        /// </summary>
        public abstract void ConfigureViewBagCurrentController();

        /// <summary>
        /// Configura o filtro para a visualização no ViewData.
        /// </summary>
        public void ConfigureViewDataFilter()
        {
            SetViewData("Filter", _paginationService.GetPageInfo().PageRequestInfo.Filter);
        }

        /// <summary>
        /// Cria mensagens de dados temporários para notificações.
        /// </summary>
        public void CreateTempDataMessages()
        {
            SetTempData("Notifications", JsonSerializer.Serialize(_messageModel.Notificacoes));
        }

        /// <summary>
        /// Adiciona uma mensagem de notificação ao ViewBag.
        /// </summary>
        /// <param name="message">A mensagem de notificação a ser adicionada.</param>
        public void AddNotificationMessage(string message)
        {
            SetViewBag("NotificationMessage", message);
        }

        /// <summary>
        /// Método abstrato para obter a mensagem de exclusão.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém a mensagem de exclusão.</returns>
        [HttpGet]
        public abstract Task<string> ObterMensagemExclusao();

        /// <summary>
        /// Método auxiliar para definir dados no ViewData.
        /// </summary>
        /// <param name="key">A chave do ViewData.</param>
        /// <param name="value">O valor a ser definido.</param>
        private void SetViewData(string key, object value)
        {
            ViewData[key] = value;
        }

        /// <summary>
        /// Método auxiliar para definir dados no TempData.
        /// </summary>
        /// <param name="key">A chave do TempData.</param>
        /// <param name="value">O valor a ser definido.</param>
        private void SetTempData(string key, object value)
        {
            TempData[key] = value;
        }

        /// <summary>
        /// Método auxiliar para definir dados no ViewBag.
        /// </summary>
        /// <param name="key">A chave do ViewBag.</param>
        /// <param name="value">O valor a ser definido.</param>
        private void SetViewBag(string key, object value)
        {
            ViewBag[key] = value;
        }
    }
}