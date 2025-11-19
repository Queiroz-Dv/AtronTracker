namespace Shared.Application.DTOS.Common
{
    /// <summary>
    /// Classe que define a estrutura das rotas de acesso
    /// </summary>
    public class RotaDeAcesso
    {
        public string Protocolo { get; set; }       // Método (https, http)

        public string Url { get; set; }          // URL base           
    }
}