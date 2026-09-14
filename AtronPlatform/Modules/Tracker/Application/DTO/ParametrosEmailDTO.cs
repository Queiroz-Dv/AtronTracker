using Shared.Application.Email.Models;

namespace Application.DTO
{
    public class ParametrosEmailDTO
    {
        public ParametrosEmailDTO()
        {
            
        }

        public ParametrosEmailDTO(string link, string identificador, int validade = 24)
        {
            Link = link;
            Identificador = identificador;
            Validade = validade;
        }

        public string Email { get; set; } 
        public string Codigo { get; set; }
        public string UsuarioNome { get; set; }
        public string Destinatario { get; set; }

        // Compatibility with templates that use different token names
        public string Nome => UsuarioNome;
        public int ValidadeHoras => Validade;

        [EmailTemplateUrl]
        public string Link { get; set; } 
        public string Identificador { get; set; } 
        public int Validade { get; set; } 

        public void VincularDadosDeEnvio(string email, string usuarioNome, string codigo = null)
        {
            Destinatario = email;
            Codigo = codigo;
            UsuarioNome = usuarioNome;
        }
    }
}