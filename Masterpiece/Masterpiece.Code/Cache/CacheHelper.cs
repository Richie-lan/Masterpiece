using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    ///缓存帮助类，客户端调用缓存都使用该帮助类
    /// </summary>
    public class CacheHelper
    {
        static CacheHelper()
        {
            InitCacheMaping();
            InitCacheDuration();
            InitCacheDependency();
        }

        private static void InitCacheMaping()
        {
            foreach (FieldInfo memberInfo in typeof(CacheKeyEnum).GetFields())
            {
                if (memberInfo.FieldType.Name == "CacheKeyEnum")
                {
                    CacheKeyEnum cacheKey = (CacheKeyEnum)Enum.Parse(typeof(CacheKeyEnum), memberInfo.Name);

                    IList<CacheKeyTypeAttribue> cacheKeyTypeAttribues = memberInfo.GetCustomAttributes<CacheKeyTypeAttribue>().ToList();
                    if (cacheKeyTypeAttribues != null && cacheKeyTypeAttribues.Count() > 0)
                    {
                        CacheStaticDomain.Instance.CacheTypeMapping[cacheKey] = cacheKeyTypeAttribues[0].CacheTypeEnum;
                    }
                    else
                    {
                        CacheStaticDomain.Instance.CacheTypeMapping[cacheKey] = CacheTypeEnum.Redis;
                    }
                }
            }
        }

        private static void InitCacheDuration()
        {
            foreach (FieldInfo memberInfo in typeof(CacheKeyEnum).GetFields())
            {
                if (memberInfo.FieldType.Name == "CacheKeyEnum")
                {
                    CacheKeyEnum cacheKey = (CacheKeyEnum)Enum.Parse(typeof(CacheKeyEnum), memberInfo.Name);

                    IList<CacheDurationAttibute> cacheDurationAttibutes = memberInfo.GetCustomAttributes<CacheDurationAttibute>().ToList();
                    if (cacheDurationAttibutes != null && cacheDurationAttibutes.Count() > 0)
                    {
                        CacheStaticDomain.Instance.CacheDurations[cacheKey] = cacheDurationAttibutes[0].CacheDuration;
                    }
                    else
                    {
                        CacheStaticDomain.Instance.CacheDurations[cacheKey] = 3600;
                    }
                }
            }
        }

        private static void InitCacheDependency()
        {
            foreach (FieldInfo memberInfo in typeof(CacheKeyEnum).GetFields())
            {
                if (!memberInfo.FieldType.Name.Equals("CacheKeyEnum"))
                {
                    continue;
                }
                IList<CacheDependencyAttibute> cacheDimensionAttibutes = memberInfo.GetCustomAttributes<CacheDependencyAttibute>().ToList();

                if (cacheDimensionAttibutes != null && cacheDimensionAttibutes.Count() > 0)
                {
                    foreach (CacheDependencyAttibute cacheDimensionAttribute in cacheDimensionAttibutes)
                    {
                        if (cacheDimensionAttribute != null)
                        {
                            foreach (KeyValuePair<CacheDependencyEnum, CacheDependencyActionType[]> cacheDependency in
                                cacheDimensionAttribute.CacheDependencys)
                            {
                                foreach (CacheDependencyActionType actionType in cacheDependency.Value)
                                {
                                    string key = cacheDependency.Key + "_" + actionType;

                                    if (!CacheStaticDomain.Instance.CacheDependencys.ContainsKey(key))
                                    {
                                        CacheStaticDomain.Instance.CacheDependencys.Add(key, new List<CacheKeyEnum>());
                                    }

                                    CacheKeyEnum cacheKey = (CacheKeyEnum)Enum.Parse(typeof(CacheKeyEnum), memberInfo.Name);
                                    if (!CacheStaticDomain.Instance.CacheDependencys[key].Contains(cacheKey))
                                    {
                                        CacheStaticDomain.Instance.CacheDependencys[key].Add(cacheKey);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //public static void SetCache<T>(T cacheObj, CacheKeyEnum cacheKey, object id = null) where T : class
        //{
        //    if (cacheObj != null)
        //    {
        //        CacheFactory.GetCacheHandler<T>(cacheKey).SetCache(cacheObj, cacheKey, id);
        //    }
        //}

        public static void SetCache<T>(T cacheObj, CacheKeyEnum cacheKey,DateTime cacheTime, object id = null) where T : class
        {
            if (cacheObj != null)
            {
                CacheFactory.GetCacheHandler<T>(cacheKey).SetCache(cacheObj, cacheKey, cacheTime, id);
            }
        }

        public static void SetCache<T>(T cacheObj, CacheKeyEnum cacheKey, DateTime cacheTime, params object[] param) where T : class
        {
            SetCache<T>(cacheObj, cacheKey, cacheTime, 0, param);
        }

        public static void SetCache<T>(T cacheObj, CacheKeyEnum cacheKey, DateTime cacheTime, object id, params object[] param) where T : class
        {
            if (cacheObj != null)
            {
                CacheFactory.GetCacheHandler<T>(cacheKey).SetCache(cacheObj, cacheKey, cacheTime, id, param);
            }
        }

        public static T GetCache<T>(CacheKeyEnum cacheKey, object id = null)
        {
            return CacheFactory.GetCacheHandler<T>(cacheKey).GetCache(cacheKey, id);
        }

        public static T GetCache<T>(CacheKeyEnum cacheKey, params object[] param)
        {
            return GetCache<T>(cacheKey, 0, param);
        }

        public static T GetCache<T>(CacheKeyEnum cacheKey, object id, params object[] param)
        {
            return CacheFactory.GetCacheHandler<T>(cacheKey).GetCache(cacheKey, id, param);
        }

        public static T GetCacheWithFill<T, T1>(CacheKeyEnum cacheKey, GetObjInvokeDelegate<T, T1> getObjInvokeDele, T1 id,DateTime cacheTime) where T : class
        {
            T result = GetCache<T>(cacheKey, id);
            if (result == null)
            {
                result = getObjInvokeDele(id);
                if (result != null)
                {
                    SetCache(result, cacheKey, cacheTime,id);
                }
            }
            return result;
        }

        public static T GetCacheWithFill<T>(CacheKeyEnum cacheKey,DateTime cacheTime, GetObjInvokeDelegate<T> getObjInvokeDele, params object[] param) where T : class
        {
            T result = GetCache<T>(cacheKey, param);
            if (result == null)
            {
                result = getObjInvokeDele();
                if (result != null)
                {
                    SetCache(result, cacheKey, cacheTime, param);
                }
            }
            return result;
        }

        public static T GetCacheWithFill<T, T1>(CacheKeyEnum cacheKey, GetObjInvokeDelegate<T, T1> getObjInvokeDele, T1 id, DateTime cacheTime, params object[] param) where T : class
        {
            T result = GetCache<T>(cacheKey, id, param);
            if (result == null)
            {
                result = getObjInvokeDele(id);
                if (result != null)
                {
                    SetCache(result, cacheKey, cacheTime,id, param);
                }
            }
            return result;
        }

        public static void RemoveCache(CacheKeyEnum cacheKey, object id = null)
        {
            //if (CacheStaticDomain.Instance.CacheTypeMapping[cacheKey] == CacheTypeEnum.LocalCache || CacheStaticDomain.Instance.CacheTypeMapping[cacheKey] == CacheTypeEnum.Composite)
            //{
            //    RedisCache.Instance.Set<DateTime>("AC_Refresh_" + cacheKey.ToString(), DateTime.Now, 3600);//1小时，目前所有缓存最长时间是1小时
            //}

            RedisCache.Instance.Set<DateTime>("AC_Refresh_" + cacheKey.ToString(), DateTime.Now, 3600);//1小时，目前所有缓存最长时间是1小时

            CacheFactory.GetCacheHandler<object>(cacheKey).RemoveCache(cacheKey, id);
        }


        public static void FlushCache()
        {
            foreach (FieldInfo field in typeof(CacheKeyEnum).GetFields())
            {
                if (field.FieldType == typeof(CacheKeyEnum))
                {
                    CacheKeyEnum cacheKey = (CacheKeyEnum)Enum.Parse(typeof(CacheKeyEnum), field.Name);
                    RefreshCache(cacheKey);
                }
            }
        }

        public static void RefreshCache(CacheKeyEnum cacheKey)
        {
            if (CacheKey.CacheKeyExpression[cacheKey].EndsWith("_{0}"))
            {
                RedisCache.Instance.Set<DateTime>("AC_Refresh_" + cacheKey.ToString(), DateTime.Now, 3600);//1小时，目前所有缓存最长时间是1小时
            }
            else
            {
                RemoveCache(cacheKey);
            }
        }

        public static void NotifyRefreshCache(CacheDependencyEnum cacheDependency, CacheDependencyActionType actionType, object dependencyArgument)
        {
            string key = cacheDependency.ToString() + "_" + actionType.ToString();

            if (CacheStaticDomain.Instance.CacheDependencys.ContainsKey(key)
                && CacheStaticDomain.Instance.CacheDependencys[key].Count > 0)
            {
                foreach (CacheKeyEnum cacheKey in CacheStaticDomain.Instance.CacheDependencys[key])
                {
                    int pos = cacheKey.ToString().IndexOf("_");
                    string cacheKeyPrefix = cacheKey.ToString().Substring(0, pos);

                    if (cacheKeyPrefix.Equals(cacheDependency.ToString()))
                    {
                        if (dependencyArgument == null)
                        {
                            RefreshCache(cacheKey);
                        }
                        else
                        {
                            RemoveCache(cacheKey, dependencyArgument);
                        }
                    }
                    else
                    {
                        RefreshCache(cacheKey);
                    }
                }
            }
        }

        public static void NotifyRefreshCache(CacheDependencyEnum cacheDependency, CacheDependencyActionType actionType, object dependencyModel, string tranId)
        {
            if (string.IsNullOrEmpty(tranId))
            {
                NotifyRefreshCache(cacheDependency, actionType, dependencyModel);
            }
            else
            {
                CacheNotifyTranContainer.Instance.Add(tranId, new CacheNotifyTranObject(cacheDependency, actionType, dependencyModel));
            }
        }

        public static void NotifyRefreshCacheForTranCommit(string tranId)
        {
            IList<CacheNotifyTranObject> list = CacheNotifyTranContainer.Instance.Get(tranId);
            if (list.Count == 0)
            {
                return;
            }
            foreach (CacheNotifyTranObject obj in list)
            {
                NotifyRefreshCache(obj.CacheDependency, obj.ActionType, obj.DependencyModel);
            }
        }
    }
}