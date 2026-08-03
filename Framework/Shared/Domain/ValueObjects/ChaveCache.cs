using Shared.Domain.Enums;
using Shared.Extensions;

namespace Shared.Domain.ValueObjects
{
    public sealed class ChaveCache
    {
        public ChaveCache(ECacheKeysInfo chave)
        {
            Chave = chave;
            Descricao = chave.GetDescription();
        }

        public ChaveCache(ECacheKeysInfo chave, string identificador)
        {
            Chave = chave;
            Descricao = $"{chave.GetDescription()}:{identificador}";
        }

        public ECacheKeysInfo Chave { get; }

        public string Descricao { get; }
    }
}
