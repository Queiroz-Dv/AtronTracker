namespace Shared.DTO.API
{
    /// <summary>
    /// Classe que define a estrutura das rotas de acesso
    /// </summary>
    public class RotaDeAcesso
    {
        public string Metodo { get; set; }       // Método (https, http)

        public string Url { get; set; }          // URL base

        public string ModuloDeAcesso { get; set; }       // Nome do módulo

        public string NomeDeAcesso { get; set; } // Nome do acesso ou ação (endpoint)
    }
}