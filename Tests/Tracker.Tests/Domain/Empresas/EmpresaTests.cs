using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EmpresaTests
{
    [Fact]
    public void Criar_DevePreservarCamposEManterCadastroPendenteSemResponsavel()
    {
        var endereco = new Endereco { Logradouro = " Rua de Teste " };
        var empresa = new Empresa
        {
            Codigo = " estudo-a ", NomeFantasia = " Estudos ",
            Endereco = endereco, Numero = " (11) 99999-0000 ", Email = " contato@example.test "
        };

        

        Assert.Equal(" estudo-a ", empresa.Codigo);
        Assert.Equal(" Estudos ", empresa.NomeFantasia);
        Assert.Equal(" Rua de Teste ", empresa.Endereco.Logradouro);
        Assert.Equal(" (11) 99999-0000 ", empresa.Numero);
        Assert.Equal(" contato@example.test ", empresa.Email);
        Assert.Equal(" Rua de Teste ", endereco.Logradouro);
        Assert.Equal(StatusEmpresa.Pendente, empresa.Status);
        Assert.Empty(empresa.Usuarios);
    }

    [Theory]
    [InlineData("", "Empresa", "Rua", "1", "contato@example.test")]
    [InlineData("EMP", " ", "Rua", "1", "contato@example.test")]
    [InlineData("EMP", "Empresa", "", "1", "contato@example.test")]
    [InlineData("EMP", "Empresa", "Rua", "", "contato@example.test")]
    [InlineData("EMP", "Empresa", "Rua", "1", " ")]
    public void Criar_NaoDeveExecutarValidacaoDeCampos(string codigo, string nome, string endereco, string numero, string email)
    {
        var empresa = new Empresa
        {
            Codigo = codigo, NomeFantasia = nome,
            Endereco = new Endereco { Logradouro = endereco }, Numero = numero, Email = email
        };

        Assert.Equal(codigo, empresa.Codigo);
        Assert.Equal(nome, empresa.NomeFantasia);
        Assert.Equal(endereco, empresa.Endereco.Logradouro);
        Assert.Equal(numero, empresa.Numero);
        Assert.Equal(email, empresa.Email);
    }

    [Fact]
    public void Criar_NaoDeveValidarOuTruncarCodigoMaiorQueOLimitePersistido()
    {
        var empresa = new Empresa { Codigo = new string('a', 26) };        

        Assert.Equal(new string('a', 26), empresa.Codigo);
    }

    [Theory]
    [InlineData("ANA", 11)]
    [InlineData("BRUNO", 27)]
    public void ConcluirCadastro_DeveVincularSomenteOResponsavelInformado(string codigo, int id)
    {
        var empresa = CriarEmpresa();
        var usuario = CriarUsuario(codigo, id);

        var vinculo = empresa.ConcluirCadastro(usuario);

        Assert.Same(vinculo, Assert.Single(empresa.Usuarios));
        Assert.Same(empresa, vinculo.Empresa);
        Assert.Same(usuario, vinculo.Usuario);
        Assert.Equal(id, vinculo.UsuarioId);
        Assert.Equal(codigo, vinculo.UsuarioCodigo);
        Assert.Equal(PapelUsuarioEmpresa.Responsavel, vinculo.Papel);
        Assert.Equal(StatusUsuarioEmpresa.Ativo, vinculo.Status);
        Assert.Equal(StatusEmpresa.Ativa, empresa.Status);
    }

    private static Empresa CriarEmpresa()
        => new()
        {
            Codigo = "ESTUDO", NomeFantasia = "Empresa de estudos",
            Endereco = new Endereco { Logradouro = "Rua de Teste" }, Numero = "(11) 99999-0000",
            Email = "contato@example.test"
        };

    private static Usuario CriarUsuario(string codigo, int id)
        => new(codigo, codigo, "Teste", $"{codigo.ToLowerInvariant()}@example.test", null)
        {
            Id = id,
            EmailConfirmado = true
        };
}

