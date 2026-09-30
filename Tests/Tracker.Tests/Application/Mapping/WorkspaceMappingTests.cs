using Application.DTO;
using Application.Mapping;
using Domain.Entities;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Xunit;

namespace Tracker.Tests.Application.Mapping;

public class WorkspaceMappingTests
{
    [Fact]
    public void WorkspaceMapping_ResponsavelNull_NaoDeveLancarExcecao()
    {
        var mockUsuarioMapper = new Mock<IToDtoMapper<Usuario, UsuarioDTO>>();
        var mapper = new WorkspaceMapping(mockUsuarioMapper.Object);

        var workspaceEntity = new Workspace
        {
            Id = 1,
            Codigo = "WS1",
            Descricao = "Workspace Teste",
            Responsavel = null!
        };

        var dto = mapper.MapToDto(workspaceEntity);

        Assert.NotNull(dto);
        Assert.Equal("WS1", dto.Codigo);
        Assert.Null(dto.Responsavel);
    }
}
