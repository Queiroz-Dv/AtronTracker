namespace Atron.Tracker.Domain.Entities
{

    public class Salario : EntityBase
    {
        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }
        public Usuario Usuario { get; set; }

        public int SalarioMensal { get; set; }

        public string? Ano { get; set; }

        public int MesId { get; set; }
    }
}