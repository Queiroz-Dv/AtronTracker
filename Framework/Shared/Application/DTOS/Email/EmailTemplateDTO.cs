namespace Shared.Application.DTOS.Email
{
    public class EmailTemplateDTO
    {
        public EmailTemplateDTO()
        {
        }

        public EmailTemplateDTO(string arquivo, string assunto, string titulo)
        {
            Arquivo = arquivo;
            Assunto = assunto;
            Titulo = titulo;
        }

        public string Arquivo { get; set; }
        public string Assunto { get; set; }
        public string Titulo { get; set; }
    }
}