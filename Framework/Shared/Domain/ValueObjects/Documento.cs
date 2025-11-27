namespace Shared.Domain.ValueObjects
{
    public class Documento
    {
        public Documento(string dado)
        {
            Dado = dado;
        }

        public Documento()
        {
            
        }

        public string Dado { get; set; }
    }
}