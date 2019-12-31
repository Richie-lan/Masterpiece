using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public interface ICacheDependencyModelHandler
    {
        int PopulateModel(CacheDependencyEnum cacheDimension, CacheKeyEnum cacheKey, CacheDependencyModel model);
    }
    public class CacheDependencyModelHandler : ICacheDependencyModelHandler
    {
        public int PopulateModel(CacheDependencyEnum cacheDimension, CacheKeyEnum cacheKey, CacheDependencyModel model)
        {
            throw new NotImplementedException();
        }
    }
}
