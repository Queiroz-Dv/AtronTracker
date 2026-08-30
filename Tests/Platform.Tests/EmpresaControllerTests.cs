using Application.DTO;
using Application.Interfaces.Services;
using AtronPlatform.WebApi.Controllers.Tracker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authorization;
using Shared.Domain.ValueObjects;
using System.Reflection;
using Xunit;

namespace Platform.Tests;

public sealed class EmpresaControllerTests
{
    [Fact]
    public void Controller_DeveExigirPolicyDoModuloEmpresa()
    {
        var authorize = typeof(EmpresaController)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(ModuloPolicies.Empresa, authorize.Policy);
    }

    [Fact]
    public async Task Post_QuandoSucesso_DeveRetornarMensagens()
    {
        var service = new EmpresaServiceFake
        {
            ResultadoCriacao = Resultado<EmpresaDTO>
                .Sucesso(new EmpresaDTO { Codigo = "emp" })
                .AdicionarMensagem("Empresa salva.")
        };
        var controller = new EmpresaController(service);

        var action = await controller.Post(new EmpresaDTO { Codigo = "emp" });

        var ok = Assert.IsType<OkObjectResult>(action);
        var mensagem = Assert.Single(
            Assert.IsAssignableFrom<IEnumerable<NotificationMessage>>(ok.Value));
        Assert.Equal("Empresa salva.", mensagem.Descricao);
    }

    [Fact]
    public async Task Put_QuandoSucesso_DeveRetornarMensagens()
    {
        var service = new EmpresaServiceFake
        {
            ResultadoAtualizacao = Resultado<EmpresaDTO>
                .Sucesso(new EmpresaDTO { Codigo = "emp" })
                .AdicionarMensagem("Empresa atualizada.")
        };
        var controller = new EmpresaController(service);

        var action = await controller.Put("emp", new EmpresaDTO { Codigo = "emp" });

        var ok = Assert.IsType<OkObjectResult>(action);
        var mensagem = Assert.Single(
            Assert.IsAssignableFrom<IEnumerable<NotificationMessage>>(ok.Value));
        Assert.Equal("Empresa atualizada.", mensagem.Descricao);
    }

    [Fact]
    public async Task Put_QuandoCodigoMudaApenasCapitalizacao_DeveRejeitar()
    {
        var controller = new EmpresaController(new EmpresaServiceFake());

        var action = await controller.Put("EMP", new EmpresaDTO { Codigo = "emp" });

        Assert.IsType<BadRequestObjectResult>(action);
    }

    private sealed class EmpresaServiceFake : IEmpresaService
    {
        public Resultado<EmpresaDTO> ResultadoCriacao { get; init; }
            = Resultado<EmpresaDTO>.Falha("Não configurado.");

        public Resultado<EmpresaDTO> ResultadoAtualizacao { get; init; }
            = Resultado<EmpresaDTO>.Falha("Não configurado.");

        public Task<Resultado<EmpresaDTO>> CriarAsync(EmpresaDTO empresa)
            => Task.FromResult(ResultadoCriacao);

        public Task<Resultado<IReadOnlyList<EmpresaDTO>>> ObterTodosAsync()
            => Task.FromResult(Resultado<IReadOnlyList<EmpresaDTO>>.Sucesso([]));

        public Task<Resultado<EmpresaDTO>> ObterPorCodigoAsync(string codigo)
            => Task.FromResult(Resultado<EmpresaDTO>.Falha("Não configurado."));

        public Task<Resultado<EmpresaDTO>> AtualizarAsync(string codigo, EmpresaDTO empresa)
            => Task.FromResult(ResultadoAtualizacao);

        public Task<Resultado> RemoverAsync(string codigo)
            => Task.FromResult(Resultado.Falha("Não configurado."));
    }
}
