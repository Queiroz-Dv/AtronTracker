namespace Application.DTO.Request
{
    public class SalarioRequest
    {
        public string UsuarioCodigo { get; set; }

        public int MesId { get; set; }

        public string Ano { get; set; }

        public int SalarioMensal { get; set; }
    }
}