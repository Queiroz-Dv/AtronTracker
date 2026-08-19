namespace Application.Records.Tarefa;

public record UsuarioRecord
{
    public string UsuarioCodigo { get; set; }
    public string Nome { get; set; }
    public string Sobrenome { get; set; }

    public string CodigoDepartamento { get; set; }
    public string DescricaoDepartamento { get; set; }

    public string CodigoCargo { get; set; }
    public string DescricaoCargo { get; set; }
}
