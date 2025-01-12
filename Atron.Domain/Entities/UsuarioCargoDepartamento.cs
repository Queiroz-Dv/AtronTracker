namespace Atron.Domain.Entities
{
    public class UsuarioCargoDepartamento
    {
        public int UsuarioId { get; set; }


        public string UsuarioCodigo { get; set; }

        public int CargoId { get; set; }


        public string CargoCodigo { get; set; }

        public int DepartamentoId { get; set; }


        public string DepartamentoCodigo { get; set; }

        // Navegação
        public Usuario Usuario { get; set; }
        public Cargo Cargo { get; set; }
        public Departamento Departamento { get; set; }
    }
}