using PersonalTracking.Notification.Models;
using System.Collections.Generic;

namespace PersonalTracking.Factory.Interfaces
{
    public interface IModelFactory<TModel, TEntity>
    {
        /// <summary>
        /// Internal notification's list
        /// </summary>
        List<NotificationMessage> Notifications { get; set; }

        /// <summary>
        /// Convert a model to a database entity
        /// </summary>
        /// <param name="model">A model type</param>
        /// <returns>An entity validated and setted</returns>
        TEntity ToEntity(TModel model);

        /// <summary>
        /// Convert an entity database to a model type
        /// </summary>
        /// <param name="entity">An entity type</param>
        /// <returns>A model</returns>
        TModel ToModel(TEntity entity);

        /// <summary>
        /// Method to validate and set a model
        /// </summary>
        /// <param name="model"> A model type</param>
        /// <returns>A model validated and setted</returns>
        TModel SetAndValidateModel(TModel model);

        TModel SetModelWhithoutValidation(TModel model);
    }
}