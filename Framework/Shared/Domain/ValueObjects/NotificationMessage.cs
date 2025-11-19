namespace Shared.Domain.ValueObjects
{
    [Serializable]
    public class NotificationMessage
    {
        public string Descricao { get; set; }

        public string Nivel { get; set; }       
    }
}