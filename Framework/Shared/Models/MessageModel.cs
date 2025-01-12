using Shared.Enums;
using Shared.Services;

namespace Shared.Models
{
    //TODO: Verificar a criação de uma classe de extensão para os serviços de notificações
    [Serializable]
    public abstract class MessageModel<Entity> : MessageService
    {
        public override void AddError(string description)
        {
            AddNotification(description, MessageLevel.Error);
        }

        public override void AddMessage(string description)
        {
            AddNotification(description, MessageLevel.Message);
        }

        public override void AddSuccessMessage(string moduleName)
        {
            AddMessage($"{moduleName} salvo com sucesso.");
        }

        public override void AddUpdateMessage(string moduleName)
        {
            AddMessage($"{moduleName} atualizado com sucesso.");
        }

        public override void AddRegisterNotFoundMessage(string moduleName)
        {
            AddError($"{moduleName} não encontrado.");
        }

        public override void AddRegisterRemovedSuccessMessage(string moduleName)
        {
            AddMessage($"{moduleName} removido com sucesso");
        }

        public override void AddRegisterInvalidMessage(string moduleName)
        {
            AddError($"{moduleName} inválido");
        }

        public override void AddRegisterExistMessage(string moduleName)
        {
            AddError($"{moduleName} já existe.");
        }

        public override void AddWarning(string description)
        {
            AddNotification(description, MessageLevel.Warning);
        }

        public abstract void Validate(Entity entity);
    }
}