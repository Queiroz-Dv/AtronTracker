using Domain.Enums;

namespace Application.DTO.Response
{
    public sealed record EmpresaResponse(
        int Id,
        string Codigo,
        string NomeFantasia,
        EnderecoEmpresaResponse Endereco,
        string Numero,
        string Email,
        StatusEmpresa Status,
        PapelUsuarioEmpresa PapelUsuario);

    public sealed record EnderecoEmpresaResponse(string Logradouro);
}
