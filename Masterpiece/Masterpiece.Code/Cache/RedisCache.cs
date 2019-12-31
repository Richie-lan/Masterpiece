using ServiceStack.Redis;
using System;
using System.Configuration;
using System.Data;

namespace Masterpiece.Code.Cache
{
    #region useless

    //public class RedisCacheAdvanced
    //{
    //    private readonly static Lazy<ConnectionMultiplexer> instance = new Lazy<ConnectionMultiplexer>(() => Init());

    //    private static ConnectionMultiplexer conn = null;
    //    //private static IDatabase db = null;
    //    private RedisCacheAdvanced()
    //    {
    //    }

    //    private static ConnectionMultiplexer Init()
    //    {
    //        if (conn == null)
    //        {
    //            string connstr = ConfigurationManager.AppSettings["Redis"];
    //            conn = ConnectionMultiplexer.Connect(connstr);
    //            //db = conn.GetDatabase();
    //        }

    //        return conn;
    //    }

    //    public static ConnectionMultiplexer Instance
    //    {
    //        get
    //        {
    //            return instance.Value;
    //        }
    //    }
    //}

    #endregion useless

    public sealed class RedisCache
    {
        #region /*字段属性*/

        //private static int timeOutdefault = int.Parse(AppSettingManager.AppSettings["RedisCacheDefault"]);
        private const int timeOutdefault = 60;

        private static readonly object _locker = new object();
        private static volatile RedisCache _cacheProvider;

        #endregion /*字段属性*/

        public static RedisCache Instance
        {
            get
            {
                if (_cacheProvider == null)
                {
                    lock (_locker)
                    {
                        if (_cacheProvider == null)
                        {
                            _cacheProvider = new RedisCache();
                        }
                    }
                }
                return _cacheProvider;
            }
        }

        public void Expire(string key, int seconds)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                Redis.ExpireEntryIn(key, new TimeSpan(0, 0, seconds));
            }
        }

        /// <summary>
        /// 清空的有缓存数据
        /// </summary>
        public void FlushAll()
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                Redis.FlushAll();
            }
        }

        /// <summary>
        /// 获取指定类型缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                return Redis.Get<T>(key);
            }
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                Redis.Remove(key);
            }
        }

        /// <summary>
        /// 添加指定类型缓存对象--默认一分钟
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void Set<T>(string key, T t)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                Redis.Set<T>(key, t, DateTime.Now.AddSeconds(timeOutdefault));
            }
        }

        /// <summary>
        /// 添加指定类型缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeOut">秒</param>
        public void Set<T>(string key, T t, int timeOut)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                timeOut = timeOut == 0 ? timeOutdefault : timeOut;
                Redis.Set<T>(key, t, DateTime.Now.AddSeconds(timeOut));
            }
        }

        public void EnQueue(string listId, string value)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                Redis.EnqueueItemOnList(listId, value);
            }
        }
        public string DeQueue(string key)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                return Redis.DequeueItemFromList(key);
            }
        }

        public long Increment(string key, int num = 1)
        {
            using (IRedisClient Redis = RedisManager.GetClient())
            {
                return Redis.IncrementValueBy(key, num);
            }
        }
    }

    public class RedisConfigInfoSection : ConfigurationSection
    {
        [ConfigurationProperty("AutoStart")]
        public bool AutoStart
        {
            get { return bool.Parse(string.IsNullOrEmpty(this["AutoStart"].ToString()) ? "true" : this["AutoStart"].ToString()); }
            set { this["AutoStart"] = value; }
        }

        [ConfigurationProperty("DB")]
        public int DB
        {
            get { return (int)this["DB"]; }
            set { this["DB"] = value; }
        }

        [ConfigurationProperty("MaxReadPoolSize")]
        public int MaxReadPoolSize
        {
            get { return int.Parse(string.IsNullOrEmpty(this["MaxReadPoolSize"].ToString()) ? "20" : this["MaxReadPoolSize"].ToString()); }
            set { this["MaxReadPoolSize"] = value; }
        }

        [ConfigurationProperty("MaxWritePoolSize")]
        public int MaxWritePoolSize
        {
            get { return int.Parse(string.IsNullOrEmpty(this["MaxWritePoolSize"].ToString()) ? "20" : this["MaxWritePoolSize"].ToString()); }
            set { this["MaxWritePoolSize"] = value; }
        }

        [ConfigurationProperty("ReadServerList")]
        public string ReadServerList
        {
            get { return (string)this["ReadServerList"]; }
            set { this["ReadServerList"] = value; }
        }

        [ConfigurationProperty("WriteServerList")]
        public string WriteServerList
        {
            get { return (string)this["WriteServerList"]; }
            set { this["WriteServerList"] = value; }
        }
    }

    public class RedisManager
    {
        private static PooledRedisClientManager prcm;

        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static RedisConfigInfoSection redisConfigInfo;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisManager()
        {
            CreateManager();
        }

        public static void Dispose()
        {
            if (prcm != null)
            {
                prcm.Dispose();
            }
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IRedisClient GetClient()
        {
            if (prcm == null)
            {
                CreateManager();
            }
            return prcm.GetClient();
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static void CreateManager()
        {
            redisConfigInfo = RedisConfigs.GetConfigInfo();
            string[] writeServerList = redisConfigInfo.WriteServerList.Split(',');
            string[] readServerList = redisConfigInfo.ReadServerList.Split(',');

            prcm = new PooledRedisClientManager(readServerList, writeServerList,
                             new RedisClientManagerConfig
                             {
                                 MaxWritePoolSize = redisConfigInfo.MaxWritePoolSize,
                                 MaxReadPoolSize = redisConfigInfo.MaxReadPoolSize,
                                 AutoStart = redisConfigInfo.AutoStart
                             });
        }
    }

    internal class RedisConfigs
    {
        private static RedisConfigInfoSection _config;

        public static RedisConfigInfoSection GetConfigInfo()
        {
            try
            {
                if (_config == null)
                {
                    _config = ConfigurationManager.GetSection("redis") as RedisConfigInfoSection;
                }
            }
            catch
            {
                throw new NoNullAllowedException("redis配置不可为空");
            }
            return _config;
        }
    }
}