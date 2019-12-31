using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public static class RedisCacheExt
    {
        //public static void Set<T>(this IDatabase db, string key, T obj, int second = 60)
        //{
        //    db.StringSet(key, JsonConvert.SerializeObject(obj), new TimeSpan(DateTime.Now.AddSeconds(second).Ticks));
        //}

        //public static void Set<T>(this ConnectionMultiplexer conn, string key, T obj, int second = 60)
        //{
        //    conn.GetDatabase().StringSet(key, JsonConvert.SerializeObject(obj), new TimeSpan(DateTime.Now.AddSeconds(second).Ticks));
        //}

        //public static T Get<T>(this IDatabase db, string key)
        //{
        //    string json = db.StringGet(key);
        //    if (string.IsNullOrEmpty(json))
        //    {
        //        return default(T);
        //    }
        //    else
        //    {
        //        return JsonConvert.DeserializeObject<T>(json);
        //    }
        //}

        //public static T Get<T>(this ConnectionMultiplexer conn, string key)
        //{
        //    string json = conn.GetDatabase().StringGet(key);
        //    if (string.IsNullOrEmpty(json))
        //    {
        //        return default(T);
        //    }
        //    else
        //    {
        //        return JsonConvert.DeserializeObject<T>(json);
        //    }
        //}

        //public static bool Exists(this IDatabase db, string key)
        //{
        //    return db.KeyExists(key);
        //}

        //public static void Remove(this IDatabase db, string key)
        //{
        //    db.KeyDelete(key);
        //}

        //public static void Remove(this ConnectionMultiplexer conn, string key)
        //{
        //    conn.GetDatabase().KeyDelete(key);
        //}
    }
}
