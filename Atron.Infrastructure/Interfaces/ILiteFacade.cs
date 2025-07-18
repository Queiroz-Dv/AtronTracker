namespace Atron.Infrastructure.Interfaces
{    public interface ILiteFacade
    {
        ILiteUnitOfWork LiteUnitOfWork { get; }
        ILiteDbContext LiteDbContext { get; }
    }
}