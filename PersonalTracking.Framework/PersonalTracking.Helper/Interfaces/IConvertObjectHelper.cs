using System;
using System.Collections.Generic;

namespace PersonalTracking.Helper.Interfaces
{
    public interface IConvertObjectHelper
    {
        /// <summary>
        /// Converte uma lista de objetos de entrada do tipo TInput em uma lista de objetos de saída do tipo TOutput usando a função de conversão fornecida.
        /// </summary>
        /// <typeparam name="TInput">O tipo dos objetos de entrada da lista.</typeparam>
        /// <typeparam name="TOutput">O tipo dos objetos de saída da lista.</typeparam>
        /// <param name="inputList">A lista de objetos de entrada a ser convertida.</param>
        /// <param name="conversionFunc">A função de conversão que realiza a conversão de cada objeto de entrada.</param>
        /// <returns>A lista de objetos de saída convertida do tipo TOutput.</returns>
        List<TOutput> ConvertList<TInput, TOutput>(List<TInput> inputList, Func<TInput, TOutput> conversionFunc);

        /// <summary>
        /// Converte um objeto de entrada do tipo TInput em um objeto de saída do tipo TOutput usando a função de conversão fornecida.
        /// </summary>
        /// <typeparam name="TInput">O tipo do objeto de entrada.</typeparam>
        /// <typeparam name="TOutput">O tipo do objeto de saída.</typeparam>
        /// <param name="input">O objeto de entrada a ser convertido.</param>
        /// <param name="conversionFunc">A função de conversão que realiza a conversão.</param>
        /// <returns>O objeto de saída convertido do tipo TOutput.</returns>
        TOutput ConvertObject<TInput, TOutput>(TInput input, Func<TInput, TOutput> conversionFunc);
    }
}