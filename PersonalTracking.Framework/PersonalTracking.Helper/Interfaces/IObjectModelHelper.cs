using System;
using System.Collections.Generic;

namespace PersonalTracking.Helper.Interfaces
{
    public interface IObjectModelHelper<Model, Entity>
        where Model : class
        where Entity : class
    {
        Entity CreateEntity(Model model, Func<Model, Entity> conversionFunction);

        Model CreateModel(Entity entity, Func<Entity, Model> conversionFunction);

        IList<Model> CreateListModels(IList<Entity> entities, Func<Entity, Model> conversionFunction);
    }
}
