namespace Shared.DTO.API
{
    public class RotaDeAcesso
    {
        public string Metodo { get; set; }       // Método (https, http)

        public string Url { get; set; }          // URL base

        public string Modulo { get; set; }       // Nome do módulo

        public string NomeDeAcesso { get; set; } // Nome do acesso ou ação (endpoint)
    }
}