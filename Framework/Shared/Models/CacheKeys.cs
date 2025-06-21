using Shared.Extensions;
using System.ComponentModel;

namespace Shared.Models
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

    public enum ECacheKeysInfo
    {
        [Description("acesso")]
        Acesso,
        [Description("sessao")]
        Sessao,
        [Description("tokenInfo")]
        TokenInfo
    }
}