using NTF.Entities;
using NTF.ErrorsType;

namespace BLL.NotificationCommand
{
    public abstract class Command
    {
        protected Command(object entity) => Entity = (EntityModel)entity;

        protected EntityModel Entity;

        protected Error Errors => Entity.Errors;
    }
}
