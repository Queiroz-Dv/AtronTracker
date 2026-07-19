using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects
{
    public class CacheInfo<T>
    {
        public CacheInfo(ECacheKeysInfo keyInfo, string value)
            : this(new ChaveCache(keyInfo, value))
        {
        }

        public CacheInfo(ChaveCache chaveCache)
        {
            Key = chaveCache.Chave;
            KeyDescription = chaveCache.Descricao;
        }

        public ECacheKeysInfo Key { get; set; }

        public string KeyDescription { get; set; }

        public T EntityInfo { get; set; }

        public DateTime ExpireTime { get; set; }
    }
}
