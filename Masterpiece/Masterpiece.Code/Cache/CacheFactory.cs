using Masterpiece.Code.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    /// 缓存操作工厂
    /// </summary>

    public class CacheFactory
    {
        public static CacheHandler<T> GetCacheHandler<T>(CacheKeyEnum cacheKey)
        {
            CacheTypeEnum cacheType = CacheStaticDomain.Instance.CacheTypeMapping[cacheKey];

            if (cacheType == CacheTypeEnum.Redis)
            {
                return new RedisCacheHandler<T>();
            }
            else if (cacheType == CacheTypeEnum.LocalCache)
            {
                return new LocalCacheHandler<T>();
            }
            else if (cacheType == CacheTypeEnum.Composite)
            {
                return new DepositeCacheHandler<T>();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public abstract class CacheHandler<T>
    {
        protected static object lockObj = new object();

        public T GetCache(CacheKeyEnum cacheKey, object id = null)
        {
            if (!CacheStaticDomain.Instance.CacheEnableSetting.ContainsKey(cacheKey)
                || CacheStaticDomain.Instance.CacheEnableSetting[cacheKey] == CacheStatusEnum.Enable)
            {
                CacheObject<T> cacheObj = GetCacheObject(cacheKey, id);

                if (cacheObj == null)
                {
                    return default(T);
                }

                DateTime refreshTime = RedisCache.Instance.Get<DateTime>("LZ_Refresh_" + cacheKey.ToString());
                if (cacheObj.CacheTime < refreshTime)
                {
                    RemoveCache(cacheKey, id);
                    return default(T);
                }

                return cacheObj.Data;
            }
            else
            {
                RemoveCache(cacheKey, id);
                return default(T);
            }
        }

        protected abstract CacheObject<T> GetCacheObject(CacheKeyEnum cacheKey, object id);

        public T GetCache(CacheKeyEnum cacheKey, object id, params object[] param)
        {
            if (!CacheStaticDomain.Instance.CacheEnableSetting.ContainsKey(cacheKey)
                || CacheStaticDomain.Instance.CacheEnableSetting[cacheKey] == CacheStatusEnum.Enable)
            {

                Dictionary<string, CacheObject<T>> dictCache = GetDictCache(cacheKey, id);

                if (dictCache != null && param != null && param.Length > 0)
                {
                    lock (lockObj)
                    {
                        string key = String.Join(",", param);
                        if (dictCache.ContainsKey(key))
                        {
                            DateTime refreshTime = RedisCache.Instance.Get<DateTime>("LZ_Refresh_" + cacheKey.ToString());

                            if (dictCache[key].CacheTime < refreshTime)
                            {
                                //RemoveCache(cacheKey, id);
                                return default(T);
                            }
                            return dictCache[key].Data;
                        }
                    }
                }
                return default(T);
            }

            else
            {
                RemoveCache(cacheKey, id);
                return default(T);
            }
        }

        protected abstract Dictionary<string, CacheObject<T>> GetDictCache(CacheKeyEnum cacheKey, object id);

        public void SetCache(T cacheObj, CacheKeyEnum cacheKey,DateTime cacheTime, object id = null)
        {
            if (!CacheStaticDomain.Instance.CacheEnableSetting.ContainsKey(cacheKey)
                || CacheStaticDomain.Instance.CacheEnableSetting[cacheKey] == CacheStatusEnum.Enable)
            {
                CacheObject<T> obj = new CacheObject<T>(cacheObj,cacheTime);
                SetCacheObject(obj, cacheKey, id);
            }
        }

        protected abstract void SetCacheObject(CacheObject<T> cacheObj, CacheKeyEnum cacheKey, object id = null);

        public void SetCache(T cacheObj, CacheKeyEnum cacheKey, DateTime cacheTime, object id = null, params object[] param)
        {
            if (!CacheStaticDomain.Instance.CacheEnableSetting.ContainsKey(cacheKey)
                || CacheStaticDomain.Instance.CacheEnableSetting[cacheKey] == CacheStatusEnum.Enable)
            {
                Dictionary<string, CacheObject<T>> dictCache = GetDictCache(cacheKey, id);

                if (dictCache == null)
                {
                    dictCache = new Dictionary<string, CacheObject<T>>();
                }

                if (param != null && param.Length > 0)
                {
                    string key = String.Join(",", param);

                    CacheObject<T> obj = new CacheObject<T>(cacheObj, cacheTime);

                    lock (lockObj)
                    {
                        dictCache[key] = obj;
                        SetDictCache(dictCache, obj, cacheKey, id);
                    }
                }
            }
        }

        protected abstract void SetDictCache(Dictionary<string, CacheObject<T>> dictCache, CacheObject<T> currentObj, CacheKeyEnum cacheKey, object id);

        public abstract void RemoveCache(CacheKeyEnum cacheKey, object id = null);

    }

    public class RedisCacheHandler<T> : CacheHandler<T>
    {
        protected override CacheObject<T> GetCacheObject(CacheKeyEnum cacheKey, object id)
        {
            return RedisCache.Instance.Get<CacheObject<T>>(CacheKey.GetCacheKey(cacheKey, id));
        }

        protected override Dictionary<string, CacheObject<T>> GetDictCache(CacheKeyEnum cacheKey, object id)
        {
            return RedisCache.Instance.Get<Dictionary<string, CacheObject<T>>>(CacheKey.GetCacheKey(cacheKey, id));
        }

        protected override void SetCacheObject(CacheObject<T> cacheObj, CacheKeyEnum cacheKey, object id = null)
        {
            RedisCache.Instance.Set<CacheObject<T>>(CacheKey.GetCacheKey(cacheKey, id), cacheObj, CacheStaticDomain.Instance.CacheDurations[cacheKey]);
        }

        protected override void SetDictCache(Dictionary<string, CacheObject<T>> dictCache, CacheObject<T> currentObj, CacheKeyEnum cacheKey, object id)
        {
            RedisCache.Instance.Set<Dictionary<string, CacheObject<T>>>(CacheKey.GetCacheKey(cacheKey, id), dictCache, CacheStaticDomain.Instance.CacheDurations[cacheKey]);
        }

        public override void RemoveCache(CacheKeyEnum cacheKey, object id = null)
        {
            RedisCache.Instance.Remove(CacheKey.GetCacheKey(cacheKey, id));
        }
    }

    public class LocalCacheHandler<T> : CacheHandler<T>
    {
        protected override CacheObject<T> GetCacheObject(CacheKeyEnum cacheKey, object id)
        {
            return (CacheObject<T>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];
        }

        protected override Dictionary<string, CacheObject<T>> GetDictCache(CacheKeyEnum cacheKey, object id)
        {
            return (Dictionary<string, CacheObject<T>>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];
        }

        protected override void SetCacheObject(CacheObject<T> cacheObj, CacheKeyEnum cacheKey, object id = null)
        {
            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), cacheObj, null,
                            DateTime.Now.AddSeconds(CacheStaticDomain.Instance.CacheDurations[cacheKey]),
                            System.Web.Caching.Cache.NoSlidingExpiration,
                            System.Web.Caching.CacheItemPriority.Default,
                            null);
        }

        protected override void SetDictCache(Dictionary<string, CacheObject<T>> dictCache, CacheObject<T> currentObj, CacheKeyEnum cacheKey, object id)
        {
            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), dictCache, null,
                         DateTime.Now.AddSeconds(CacheStaticDomain.Instance.CacheDurations[cacheKey]),
                         System.Web.Caching.Cache.NoSlidingExpiration,
                         System.Web.Caching.CacheItemPriority.Default,
                         null);
        }

        public override void RemoveCache(CacheKeyEnum cacheKey, object id = null)
        {
            HttpRuntime.Cache.Remove(CacheKey.GetCacheKey(cacheKey, id));
        }
    }

    public class DepositeCacheHandler<T> : CacheHandler<T>
    {
        protected override CacheObject<T> GetCacheObject(CacheKeyEnum cacheKey, object id)
        {
            CacheObject<T> cacheObject = (CacheObject<T>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];

            if (cacheObject == null)
            {
                cacheObject = RedisCache.Instance.Get<CacheObject<T>>(CacheKey.GetCacheKey(cacheKey, id));
            }

            return cacheObject;
        }

        protected override Dictionary<string, CacheObject<T>> GetDictCache(CacheKeyEnum cacheKey, object id)
        {
            Dictionary<string, CacheObject<T>> dict = (Dictionary<string, CacheObject<T>>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];

            if (dict == null)
            {
                dict = RedisCache.Instance.Get<Dictionary<string, CacheObject<T>>>(CacheKey.GetCacheKey(cacheKey, id));
            }

            return dict;
        }

        protected override void SetCacheObject(CacheObject<T> cacheObj, CacheKeyEnum cacheKey, object id = null)
        {
            RedisCache.Instance.Set<CacheObject<T>>(CacheKey.GetCacheKey(cacheKey, id), cacheObj, CacheStaticDomain.Instance.CacheDurations[cacheKey]);

            cacheObj.CacheTime = DateTime.Now.AddSeconds(60);

            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), cacheObj, null,
                        DateTime.Now.AddSeconds(60),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Default,
                        null);
        }

        protected override void SetDictCache(Dictionary<string, CacheObject<T>> dictCache, CacheObject<T> currentObj, CacheKeyEnum cacheKey, object id)
        {
            RedisCache.Instance.Set<Dictionary<string, CacheObject<T>>>(CacheKey.GetCacheKey(cacheKey, id), dictCache, CacheStaticDomain.Instance.CacheDurations[cacheKey]);

            currentObj.CacheTime = DateTime.Now.AddSeconds(60);

            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), dictCache, null,
                       DateTime.Now.AddSeconds(60),
                       System.Web.Caching.Cache.NoSlidingExpiration,
                       System.Web.Caching.CacheItemPriority.Default,
                       null);
        }

        public override void RemoveCache(CacheKeyEnum cacheKey, object id = null)
        {
            HttpRuntime.Cache.Remove(CacheKey.GetCacheKey(cacheKey, id));
            RedisCache.Instance.Remove(CacheKey.GetCacheKey(cacheKey, id));
        }
    }

    public class FlexibleLocalCacheHandler<T> : CacheHandler<T>
    {
        protected override CacheObject<T> GetCacheObject(CacheKeyEnum cacheKey, object id)
        {
            CacheObject<T> cacheObject = (CacheObject<T>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];

            if (cacheObject == null)
            {
                cacheObject = (CacheObject<T>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id) + "_5"];
            }

            return cacheObject;
        }

        protected override Dictionary<string, CacheObject<T>> GetDictCache(CacheKeyEnum cacheKey, object id)
        {
            Dictionary<string, CacheObject<T>> dict = (Dictionary<string, CacheObject<T>>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id)];

            if (dict == null)
            {
                dict = (Dictionary<string, CacheObject<T>>)HttpRuntime.Cache[CacheKey.GetCacheKey(cacheKey, id) + "_5"];
            }

            return dict;
        }

        protected override void SetCacheObject(CacheObject<T> cacheObj, CacheKeyEnum cacheKey, object id = null)
        {
            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), cacheObj, null,
                              DateTime.Now.AddSeconds(CacheStaticDomain.Instance.CacheDurations[cacheKey]),
                              System.Web.Caching.Cache.NoSlidingExpiration,
                              System.Web.Caching.CacheItemPriority.Default,
                              null);

            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id) + "_5", cacheObj, null,
                        DateTime.Now.AddSeconds(60 * 5),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Default,
                        null);
        }

        protected override void SetDictCache(Dictionary<string, CacheObject<T>> dictCache, CacheObject<T> currentObj, CacheKeyEnum cacheKey, object id)
        {
            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id), dictCache, null,
                  DateTime.Now.AddSeconds(CacheStaticDomain.Instance.CacheDurations[cacheKey]),
                  System.Web.Caching.Cache.NoSlidingExpiration,
                  System.Web.Caching.CacheItemPriority.Default,
                  null);

            HttpRuntime.Cache.Insert(CacheKey.GetCacheKey(cacheKey, id) + "_5", dictCache, null,
                       DateTime.Now.AddSeconds(60 * 5),
                       System.Web.Caching.Cache.NoSlidingExpiration,
                       System.Web.Caching.CacheItemPriority.Default,
                       null);
        }

        public override void RemoveCache(CacheKeyEnum cacheKey, object id = null)
        {
            HttpRuntime.Cache.Remove(CacheKey.GetCacheKey(cacheKey, id));
            HttpRuntime.Cache.Remove(CacheKey.GetCacheKey(cacheKey, id) + "_5");
        }
    }
}
