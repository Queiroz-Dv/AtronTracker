using Shared.Enums;
using Shared.Services;

namespace Shared.Models
{
    //TODO: Verificar a criação de uma classe de extensão para os serviços de notificações
    [Serializable]
    public abstract class MessageModel : MessageService
    {
        public override void AddError(string description)
        {
            AddNotification(description, Level.Error);
        }

        public override void AddMessage(string description)
        {
            AddNotification(description, Level.Message);
        }

        public override void AddSuccessMessage(string key)
        {
            AddMessage($"Registro {key} salvo com sucesso.");
        }

        public override void AddUpdateMessage(string key)
        {
            AddMessage($"Registro {key} atualizado com sucesso.");
        }

        public override void AddRegisterNotFoundMessage(string key = "")
        {
            AddError($"Registro {key} não encontrado.");
        }

        public override void AddRegisterRemovedSuccessMessage(string key)
        {
            AddMessage($"Registro {key} removido com sucesso");
        }

        public override void AddRegisterInvalidMessage(string key = "")
        {
            AddError($"Registro {key} inválido");
        }

        public override void AddRegisterExistMessage(string key)
        {
            AddError($"Registro {key} já existe.");
        }

        public override void AddWarning(string description)
        {
            AddNotification(description, Level.Warning);
        }
    }
}