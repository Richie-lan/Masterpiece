using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public class CacheNotifyTranObject
    {
        public CacheDependencyEnum CacheDependency
        {
            set;get;
        }

        public CacheDependencyActionType ActionType
        {
            set;get;
        }

        public object DependencyModel
        {
            set;get;
        }

        public CacheNotifyTranObject(CacheDependencyEnum cacheDependency, CacheDependencyActionType actionType, object dependencyModel)
        {
            CacheDependency = cacheDependency;
            ActionType = actionType;
            DependencyModel = dependencyModel;
        }
    }

    public class CacheNotifyTranContainer
    {
        private static readonly CacheNotifyTranContainer instance = new CacheNotifyTranContainer();

        public static CacheNotifyTranContainer Instance
        {
            get
            {
                return instance;
            }
        }

        private ConcurrentDictionary<string, IList<CacheNotifyTranObject>> Container = new ConcurrentDictionary<string, IList<CacheNotifyTranObject>>();

        public void Add(string tranId, CacheNotifyTranObject obj)
        {
            if (!Container.ContainsKey(tranId))
            {
                Container.TryAdd(tranId, new List<CacheNotifyTranObject>());
            }


            Container[tranId].Add(obj);
        }

        public IList<CacheNotifyTranObject> Get(string tranId)
        {
            IList<CacheNotifyTranObject> objs = new List<CacheNotifyTranObject>();

            if (Container.ContainsKey(tranId))
            {
                objs = Container[tranId];
                IList<CacheNotifyTranObject> items = null;
                Container.TryRemove(tranId, out items);
            }
         
            return objs;
        }
    }
}
