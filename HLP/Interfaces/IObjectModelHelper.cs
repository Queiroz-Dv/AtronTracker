using System;
using System.Collections.Generic;

namespace HLP.Interfaces
{
    public interface IObjectModelHelper<Model, Entity>
        where Model : class
        where Entity : class
    {
        Entity CreateEntity(Model model, Func<Model, Entity> conversionFunction);

        Model CreateModel(Entity entity, Func<Entity, Model> conversionFunction);

        IList<Model> CreateListModels(List<Entity> entities, Func<Entity, Model> conversionFunction);
    }
}
