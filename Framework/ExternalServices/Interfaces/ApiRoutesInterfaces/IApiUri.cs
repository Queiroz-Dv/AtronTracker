namespace ExternalServices.Interfaces.ApiRoutesInterfaces
{
    public interface IApiUri
    {
        public string Uri { get; set; }
        public string Modulo { get; set; }         
    }

    /// <summary>
    /// Interface padrão para as urls dos módulos
    /// </summary>
    public interface IUrlModuleFactory
    {        
        public string Url { get; set; }
    }

    /// <summary>
    /// Classe concreta de implementação das urls dos módulos
    /// </summary>
    public class UrlFactory : IUrlModuleFactory
    {      
        public string Url { get; set; }
    }
}