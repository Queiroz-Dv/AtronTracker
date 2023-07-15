using HLP.Interfaces;
using System;
using System.Collections.Generic;

namespace HLP.Entity
{

    public class ConvertObjectHelper : IConvertObjectHelper
    {
        public List<TOutput> ConvertList<TInput, TOutput>(List<TInput> inputList, Func<TInput, TOutput> conversionFunc)
        {
            IList<TOutput> outputList = new List<TOutput>();

            foreach (var input in inputList)
            {
                TOutput output = conversionFunc(input);
                outputList.Add(output);
            }

            return outputList as List<TOutput>;
        }

        public  TOutput ConvertObject<TInput, TOutput>(TInput input, Func<TInput, TOutput> conversionFunc)
        {
            return conversionFunc(input);
        }
    }
}