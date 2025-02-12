namespace Communication.Interfaces.Services
{
    /// <summary>
    /// Interface padrão para as urls dos módulos
    /// </summary>
    public interface IUrlTransferService
    {
        public string Url { get; set; }
        public string Modulo { get; set; }
    }
}