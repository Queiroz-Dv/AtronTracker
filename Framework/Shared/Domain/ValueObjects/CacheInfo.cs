using Shared.Domain.Enums;
using Shared.Extensions;

namespace Shared.Domain.ValueObjects
{
    public class CacheInfo<T>
    {
        public CacheInfo(ECacheKeysInfo keyInfo, string value)
        {
            Key = keyInfo;
            KeyDescription = string.Format($"{Key.GetDescription()}:{value}");
        }

        public ECacheKeysInfo Key { get; set; }

        public string KeyDescription { get; set; }

        public T EntityInfo { get; set; }

        public DateTime ExpireTime { get; set; }
    }
}