using HLP.Entity;
using HLP.Interfaces;
using System;
using System.Collections.Generic;

namespace HLP.Helpers
{
    /// <summary>
    /// Classe que fornece métodos para criar objetos de entidade e modelos convertidos
    /// </summary>
    /// <typeparam name="Model">Modelo a ser processado</typeparam>
    /// <typeparam name="Entity">Entidade da DAL que será processado</typeparam>
    public class ObjectModelHelper<Model, Entity> : IObjectModelHelper<Model, Entity>
        where Model : class
        where Entity : class
    {
        /// <summary>
        /// Este método recebe um objeto do tipo Model e uma função de conversão que converte o objeto Model em um objeto do tipo Entity. 
        /// </summary>
        /// <param name="model">Modelo a ser convertido</param>
        /// <param name="conversionFunction">Função de conversão a ser utilizada</param>
        /// <returns>Utiliza a classe ConvertObjectHelper para realizar a conversão e retorna o objeto Entity resultante.</returns>
        public Entity CreateEntity(Model model, Func<Model, Entity> conversionFunction)
        {
            var convertObject = ConvertObjectHelper.ConvertObject(input: model, conversionFunc: conversionFunction);
            return convertObject;
        }

        /// <summary>
        /// Este método recebe uma lista de objetos do tipo Entity e uma função de conversão que converte cada objeto Entity em um objeto do tipo Model.
        /// </summary>
        /// <param name="entities">Entidades da DAL a serem convertidas</param>
        /// <param name="conversionFunction">Função de conversão a ser utilizada</param>
        /// <returns>Utiliza a classe ConvertObjectHelper para realizar a conversão e retorna uma lista contendo os objetos Model resultantes.</returns>
        public IList<Model> CreateListModels(IList<Entity> entities, Func<Entity, Model> conversionFunction)
        {
            var objectsConverted = ConvertObjectHelper.ConvertList(inputList: entities, conversionFunc: conversionFunction);
            return objectsConverted;
        }

        /// <summary>
        /// Este método recebe um objeto do tipo Entity e uma função de conversão que converte o objeto Entity em um objeto do tipo Model.
        /// </summary>
        /// <param name="entity">Entidade da DAL a ser convertida</param>
        /// <param name="conversionFunction">Função de conversão a ser utilizada</param>
        /// <returns>Utiliza a classe ConvertObjectHelper para realizar a conversão e retorna o objeto Model resultante.</returns>
        public Model CreateModel(Entity entity, Func<Entity, Model> conversionFunction)
        {
            var convertObject = ConvertObjectHelper.ConvertObject(input: entity, conversionFunc: conversionFunction);
            return convertObject;
        }
    }
}