using Atron.Infrastructure.Interfaces;
using System;

namespace Atron.Infrastructure.Context
{
    public class LiteFacade : ILiteFacade
    {
        public LiteFacade(ILiteUnitOfWork liteUnitOfWork, ILiteDbContext liteDbContext)
        {
            LiteUnitOfWork = liteUnitOfWork ?? throw new ArgumentNullException(nameof(liteUnitOfWork));
            LiteDbContext = liteDbContext ?? throw new ArgumentNullException(nameof(liteDbContext));
        }

        public ILiteUnitOfWork LiteUnitOfWork { get; }

        public ILiteDbContext LiteDbContext { get; }
    }
}