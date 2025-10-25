namespace Shared.DTO.Email
{
    // Mensagem de e-mail
    public class EmailMessage
    {
        public List<string> To { get; set; } = new();
        public string Subject { get; set; }
        public string Body { get; set; }
    }

}
