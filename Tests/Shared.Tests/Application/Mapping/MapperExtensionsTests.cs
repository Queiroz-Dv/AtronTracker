using Shared.Application.Interfaces.Mapping;
using Xunit;

namespace Shared.Tests.Application.Mapping;

public sealed class MapperExtensionsTests
{
    [Fact]
    public void MapToUpdate_DeveAtualizarAEntidadeExistente()
    {
        var entity = new TestEntity { Id = 10, Description = "Antes" };
        var dto = new TestDto { Id = 99, Description = "Depois" };

        entity.MapToUpdate(dto, new TestUpdateMapper());

        Assert.Equal(10, entity.Id);
        Assert.Equal("Depois", entity.Description);
    }

    private sealed class TestUpdateMapper : IUpdateMapper<TestEntity, TestDto>
    {
        public void MapToUpdate(TestDto dto, TestEntity entity)
        {
            entity.Description = dto.Description;
        }
    }

    private sealed class TestEntity
    {
        public int Id { get; init; }

        public required string Description { get; set; }
    }

    private sealed class TestDto
    {
        public int Id { get; init; }

        public required string Description { get; init; }
    }
}
