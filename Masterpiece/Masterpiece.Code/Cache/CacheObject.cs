using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
     [Serializable]
    public class CacheObject<T>
    {
         public CacheObject(T data)
         {
             Data = data;
             CacheTime = DateTime.Now;
         }

        public CacheObject(T data,DateTime cacheTime)
        {
            Data = data;
            CacheTime = cacheTime;
        }

        public T Data
         {
             set;
             get;
         }

         private DateTime cacheTime;
         public DateTime CacheTime
         {
             set
             {
                 cacheTime = value;
             }
             get
             {
                 if (cacheTime == DateTime.MinValue)
                 {
                     cacheTime = DateTime.Now;
                 }
                 return cacheTime;
             }
         }
    }
}
