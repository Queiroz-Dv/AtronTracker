using Shared.Domain.Enums;

namespace Shared.Domain.ValueObjects
{
    public class CacheInfo<T>(ChaveCache chaveCache)
    {
        public CacheInfo(ECacheKeysInfo keyInfo, string value)
            : this(new ChaveCache(keyInfo, value))
        {
        }

        public ECacheKeysInfo Key { get; set; } = chaveCache.Chave;

        public string KeyDescription { get; set; } = chaveCache.Descricao;

        public T EntityInfo { get; set; }

        public DateTime ExpireTime { get; set; }

        public void VincularDadosTemporarios(T dados)
        {
            EntityInfo = dados;
        }
    }
}
