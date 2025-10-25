namespace Application.DTO
{
    public class SalarioDTO
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public int MesId { get; set; }

        public string Ano { get; set; }

        public int SalarioMensal { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public MesDTO Mes { get; set; }
    }
}