namespace Application.DTO.Response
{
    public class SalarioResponse
    {
        public int Id { get; set; }
        public int SalarioMensal { get; set; }
        public string Ano { get; set; }
        public MesDTO Mes { get; set; }
        public UsuarioRecord Usuario { get; set; }
    }
}