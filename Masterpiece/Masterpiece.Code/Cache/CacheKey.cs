using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    /// 缓存键配置类
    /// </summary>
    public class CacheKey
    {

        public static Dictionary<CacheKeyEnum, string> CacheKeyExpression = new Dictionary<CacheKeyEnum, string>();
        private static void InitCacheKey()
        {
            string cachePrefix = "AC_";
            CacheKeyExpression[CacheKeyEnum.Test_Cache] = cachePrefix + "Test_Cache_{0}";
            CacheKeyExpression[CacheKeyEnum.Product_Cache] = cachePrefix + "Product_Cache_{0}";
        }

        static CacheKey()
        {
            InitCacheKey();
        }

        public static string GetCacheKey(CacheKeyEnum cacheKeyEnum, object id)
        {
            return string.Format(CacheKeyExpression[cacheKeyEnum], id == null ? "0" : id.ToString());
        }

        public static string GetCacheKey(CacheKeyEnum cacheKeyEnum, params string[] paras)
        {
            return string.Format(CacheKeyExpression[cacheKeyEnum], paras);
        }
    }
}
