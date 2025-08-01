using Atron.Domain.Entities;

namespace Atron.Application.DTO.Response
{
    public class SalarioResponse
    {
        public int Id { get; set; }
        public int SalarioMensal { get; set; }
        public string Ano { get; set; }
        public Mes Mes { get; set; }
        public UsuarioRecord Usuario { get; set; }
    }
}