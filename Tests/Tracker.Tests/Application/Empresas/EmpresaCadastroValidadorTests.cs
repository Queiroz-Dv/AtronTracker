using Application.DTO.Request;
using Application.Mapping;
using Application.Validador;
using Domain.Entities;
using Domain.Extensions;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EmpresaCadastroValidadorTests
{
    private readonly EmpresaCadastroValidador _validador = new();

    [Theory]
    [InlineData("Codigo", 25)]
    [InlineData("NomeFantasia", 150)]
    [InlineData("Numero", 20)]
    [InlineData("Email", 254)]
    public void Validar_DeveRecusarCampoVazioOuAcimaDoLimite(string campo, int limite)
    {
        var request = RequestValido();
        typeof(EmpresaCadastroRequest).GetProperty(campo)!.SetValue(request, " ");
        Assert.NotEmpty(_validador.Validar(request));
        typeof(EmpresaCadastroRequest).GetProperty(campo)!.SetValue(request, new string('a', limite + 1));
        Assert.NotEmpty(_validador.Validar(request));
        typeof(EmpresaCadastroRequest).GetProperty(campo)!.SetValue(request, null);
        Assert.NotEmpty(_validador.Validar(request));
    }

    [Fact]
    public void Validar_DeveRecusarRequestOuEnderecoAusente()
    {
        Assert.NotEmpty(_validador.Validar(null));
        var request = RequestValido();
        request.Endereco = null!;
        Assert.NotEmpty(_validador.Validar(request));
        request.Endereco = new EnderecoEmpresaRequest { Logradouro = " " };
        Assert.NotEmpty(_validador.Validar(request));
        request.Endereco.Logradouro = new string('a', 201);
        Assert.NotEmpty(_validador.Validar(request));
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("Empresa <empresa@example.test>")]
    [InlineData(" empresa@example.test ")]
    public void Validar_DeveRecusarEmailInvalidoSemModificarEntrada(string email)
    {
        var request = RequestValido();
        request.Email = email;
        Assert.NotEmpty(_validador.Validar(request));
        Assert.Equal(email, request.Email);
    }

    [Theory]
    [InlineData(true, true, 1, "ANA")]
    [InlineData(false, false, 1, "ANA")]
    [InlineData(false, true, 0, "ANA")]
    [InlineData(false, true, 1, " ")]
    public void ValidarResponsavel_DeveRecusarContaInvalida(bool inativo, bool confirmado, int id, string codigo)
    {
        var usuario = new Usuario { Id = id, Codigo = codigo, Inativo = inativo, EmailConfirmado = confirmado };
        Assert.NotEmpty(_validador.ValidarResponsavel(usuario));
    }

    [Fact]
    public void ValidarResponsavel_DeveRecusarContaAusente()
        => Assert.NotEmpty(_validador.ValidarResponsavel(null));

    [Fact]
    public void ValidarConclusao_DeveRecusarEmpresaJaConcluidaSemTrocarResponsavel()
    {
        var empresa = new EmpresaMapping().MapToEntity(RequestValido());
        var usuario = new Usuario { Id = 1, Codigo = "ANA", EmailConfirmado = true };
        Assert.Empty(_validador.ValidarConclusao(empresa));
        empresa.ConcluirCadastro(usuario);

        Assert.NotEmpty(_validador.ValidarConclusao(empresa));
        Assert.Same(usuario, Assert.Single(empresa.Usuarios).Usuario);
    }

    [Fact]
    public void ValidarEMapear_DevePreservarOsDadosInformados()
    {
        var request = RequestValido();
        request.Codigo = "Estudo-a";
        request.NomeFantasia = " Empresa de estudos ";
        request.Endereco.Logradouro = " Rua de Teste ";

        Assert.Empty(_validador.Validar(request));
        var empresa = new EmpresaMapping().MapToEntity(request);

        Assert.Equal(request.Codigo, empresa.Codigo);
        Assert.Equal(request.NomeFantasia, empresa.NomeFantasia);
        Assert.Equal(request.Endereco.Logradouro, empresa.Endereco.Logradouro);
        Assert.Equal(request.Numero, empresa.Numero);
        Assert.Equal(request.Email, empresa.Email);
    }

    internal static EmpresaCadastroRequest RequestValido() => new()
    {
        Codigo = "Estudo", NomeFantasia = "Empresa de estudos",
        Endereco = new EnderecoEmpresaRequest { Logradouro = "Rua de Teste" },
        Numero = "(11) 99999-0000", Email = "empresa@example.test"
    };
}
