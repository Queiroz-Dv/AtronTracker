namespace Atron.Domain.Entities
{

    public class Salario : EntityBase
    {
        public Salario()
        {
            UsuarioCodigo = string.Empty;
            Usuario = new Usuario();
            Mes = new Mes();
        }

        public Salario(int usuarioId,
                       string usuarioCodigo,
                       int salarioMensal,
                       string ano,
                       int mesId)
        {
            UsuarioId = usuarioId;
            UsuarioCodigo = usuarioCodigo;
            SalarioMensal = salarioMensal;
            Ano = ano;
            UsuarioId = usuarioId;
            UsuarioCodigo = usuarioCodigo;
            MesId = mesId;
        }

        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }
        public Usuario Usuario { get; set; }

        public int SalarioMensal { get; set; }

        public string? Ano { get; set; }

        public int MesId { get; set; }
        public Mes Mes { get; set; }
    }
}