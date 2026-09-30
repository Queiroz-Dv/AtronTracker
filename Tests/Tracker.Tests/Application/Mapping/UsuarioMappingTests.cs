using Application.DTO;
using Application.Mapping;
using Domain.Entities;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Xunit;

namespace Tracker.Tests.Application.Mapping;

public class UsuarioMappingTests
{
    [Fact]
    public void UsuarioMapping_SemCargoDepartamentoEPerfil_NaoDeveLancarExcecao()
    {
        var mockCargoMapper = new Mock<IToDtoMapper<Cargo, CargoDTO>>();
        var mockDepartamentoMapper = new Mock<IToDtoMapper<Departamento, DepartamentoDTO>>();
        var mockPerfilMapper = new Mock<IToDtoMapper<PerfilDeAcesso, PerfilDeAcessoDTO>>();

        var mapper = new UsuarioMapping(mockCargoMapper.Object, mockDepartamentoMapper.Object, mockPerfilMapper.Object);

        var usuarioEntity = new Usuario
        {
            Id = 1,
            Codigo = "USR1",
            Nome = "Usuario Teste",
            UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>
            {
                new UsuarioCargoDepartamento
                {
                    CargoCodigo = "CARG1",
                    DepartamentoCodigo = "DEPT1",
                    Cargo = null!,
                    Departamento = null!
                }
            },
            PerfisDeAcessoUsuario = new List<PerfilDeAcessoUsuario>
            {
                new PerfilDeAcessoUsuario
                {
                    PerfilDeAcessoCodigo = "PERF1",
                    PerfilDeAcesso = null!
                }
            }
        };

        var dto = mapper.MapToDto(usuarioEntity);

        Assert.NotNull(dto);
        Assert.Equal("USR1", dto.Codigo);
        Assert.Equal("CARG1", dto.CargoCodigo);
        Assert.Null(dto.Cargo);
        Assert.Equal("DEPT1", dto.DepartamentoCodigo);
        Assert.Null(dto.Departamento);
        Assert.Empty(dto.PerfisDeAcesso);
    }
}
