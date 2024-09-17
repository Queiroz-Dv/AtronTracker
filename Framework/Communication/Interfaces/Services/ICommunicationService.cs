﻿using Shared.DTO;
using Shared.Interfaces;
using Shared.Models;

namespace Communication.Interfaces.Services
{
    /// <summary>
    /// Interface de comunicação dos resultados com a API
    /// </summary>
    public interface ICommunicationService : IMessages
    {

        /// <summary>
        /// Adiciona uma resultado da resposta da API
        /// </summary>
        /// <param name="resultResponse">Resultado da resposta</param>
        void AddResponseContent(ResultResponseDTO resultResponse);

        /// <summary>
        /// Adiciona uma lista de resultados de resposta da API
        /// </summary>
        /// <param name="resultResponses">Lista de resposta</param>
        void AddResponseContent(List<ResultResponseDTO> resultResponses);

        /// <summary>
        /// Método que obtém a lista de resultados da API
        /// </summary>
        /// <returns>Uma lista de resultados preenchidos internamente</returns>
        List<ResultResponseDTO> GetResultResponses();


        void AddMessage(Message message);
        void AddMessages(List<Message> messages);
        List<Message> GetMessages();
    }
}