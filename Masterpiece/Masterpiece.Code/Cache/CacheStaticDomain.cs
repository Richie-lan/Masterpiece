using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    /// 缓存系统的静态数据
    /// </summary>
    public class CacheStaticDomain
    {
        public static readonly CacheStaticDomain Instance = new CacheStaticDomain();

        public CacheStaticDomain()
        {
            CacheDependencys = new Dictionary<string, IList<CacheKeyEnum>>();
            CacheEnableSetting = new Dictionary<CacheKeyEnum, CacheStatusEnum>();
            CacheDurations = new Dictionary<CacheKeyEnum, int>();
            CacheTypeMapping = new Dictionary<CacheKeyEnum, CacheTypeEnum>();
        }

        public Dictionary<string, IList<CacheKeyEnum>> CacheDependencys
        {
            set;
            get;
        }

        public Dictionary<CacheKeyEnum, CacheStatusEnum> CacheEnableSetting
        {
            set;
            get;
        }

        public Dictionary<CacheKeyEnum, int> CacheDurations
        {
            set;
            get;
        }

        public Dictionary<CacheKeyEnum, CacheTypeEnum> CacheTypeMapping
        {
            set;
            get;
        }
    }
}
