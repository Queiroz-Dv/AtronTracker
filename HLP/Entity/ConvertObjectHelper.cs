using System;
using System.Collections.Generic;

namespace HLP.Entity
{
    /// <summary>
    /// Uma classe helper que disponibiliza métodos de conversão
    /// </summary>
    public static class ConvertObjectHelper
    {
        /// <summary>
        /// Converte uma lista de objetos para um outro tipo usando uma função de conversão
        /// </summary>
        /// <typeparam name="TInput">O tipo de objeto de entrada.</typeparam>
        /// <typeparam name="TOutput">O tipo de objeto de saída.</typeparam>
        /// <param name="inputList">A lista de objetos de entrada.</param>
        /// <param name="conversionFunc">A função que específica como converter cada objeto de entrada.</param>
        /// <returns>Uma lista de objetos de saída convertidos dos objetos de entrada.</returns>
        public static IList<TOutput> ConvertList<TInput, TOutput>(this IList<TInput> inputList, Func<TInput, TOutput> conversionFunc)
        {
            IList<TOutput> outputList = new List<TOutput>();

            foreach (var input in inputList)
            {
                TOutput output = conversionFunc(input);
                outputList.Add(output);
            }

            return outputList;
        }

        /// <summary>
        /// Converte um objeto de um tipo para outro usando uma função de conversão.
        /// </summary>
        /// <typeparam name="TInput">O tipo de objeto de entrada.</typeparam>
        /// <typeparam name="TOutput">O tipo de objeto de saída.</typeparam>
        /// <param name="input">O objeto de entrada que será convertido.</param>
        /// <param name="conversionFunc">A função que específica como converter o objeto de entrada.</param>
        /// <returns>O objeto de saída convertido do objeto de entrada.</returns>
        public static TOutput ConvertObject<TInput, TOutput>(this TInput input, Func<TInput, TOutput> conversionFunc)
        {
            return conversionFunc(input);
        }
    }
}