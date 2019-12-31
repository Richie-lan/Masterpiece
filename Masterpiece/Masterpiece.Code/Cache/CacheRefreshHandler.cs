using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public abstract class CacheRefreshHandler
    {
        public virtual void Refresh(CacheKeyEnum cacheKeyEnum, int Id) 
        { }
    }

    public class RedisCacheRefreshHandler : CacheRefreshHandler
    {
        public override void Refresh(CacheKeyEnum cacheKeyEnum,int Id)
        {
            CacheHelper.RemoveCache(cacheKeyEnum, Id);
        }
    }

    public class SqlCacheRefreshHandler : CacheRefreshHandler
    {
        public override void Refresh(CacheKeyEnum cacheKeyEnum, int Id)
        {
            throw new NotImplementedException();
        }
    }

}
