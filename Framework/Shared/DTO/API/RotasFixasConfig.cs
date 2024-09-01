namespace Shared.DTO.API
{
    /// <summary>
    /// Classe das rotas de configuração padrão da API
    /// </summary>
    public class RotasFixasConfig
    {
        /// <summary>
        /// Rota principal para obtenção das rotas do api connect
        /// </summary>
        public RotaDeAcesso RotasPrincipalDeAcessoDoConnect { get; set; }

        /// <summary>
        /// Rota principal para criação de rotas do api connect
        /// </summary>
        public RotaDeAcesso RotaDeCriacaoDoConnect { get; set; }
    }
}