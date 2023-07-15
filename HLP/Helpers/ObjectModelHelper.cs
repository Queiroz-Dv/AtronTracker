using HLP.Entity;
using HLP.Interfaces;
using System;
using System.Collections.Generic;

namespace HLP.Helpers
{
    public class ObjectModelHelper<Model, Entity> : IObjectModelHelper<Model, Entity>
        where Model : class
        where Entity : class
    {
        private IConvertObjectHelper _objectHelper;

        public ObjectModelHelper()
        {
            _objectHelper = new ConvertObjectHelper();
        }

        public Entity CreateEntity(Model model, Func<Model, Entity> conversionFunction)
        {
            var convertObject = _objectHelper.ConvertObject(model, conversionFunction);
            return convertObject;
        }

        public IList<Model> CreateListModels(List<Entity> entities, Func<Entity, Model> conversionFunction)
        {
            var objectsConverted = _objectHelper.ConvertList(entities, conversionFunction);
            return objectsConverted;
        }

        public Model CreateModel(Entity entity, Func<Entity, Model> conversionFunction)
        {
            var convertObject = _objectHelper.ConvertObject(entity, conversionFunction);
            return convertObject;
        }
    }
}