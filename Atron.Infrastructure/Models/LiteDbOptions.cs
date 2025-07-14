namespace Atron.Infrastructure.Models
{
    public class LiteDbOptions
    {
        public string DatabasePath { get; set; }
        public string Password { get; set; }  // Senha para o banco de dados LiteDB
    }
}