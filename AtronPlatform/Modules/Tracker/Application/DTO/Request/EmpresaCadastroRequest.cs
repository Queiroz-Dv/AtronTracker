namespace Application.DTO.Request
{
    public sealed class EmpresaCadastroRequest
    {
        public string Codigo { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public EnderecoEmpresaRequest Endereco { get; set; } = new();
        public string Numero { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public sealed class EnderecoEmpresaRequest
    {
        public string Logradouro { get; set; } = string.Empty;
    }
}
