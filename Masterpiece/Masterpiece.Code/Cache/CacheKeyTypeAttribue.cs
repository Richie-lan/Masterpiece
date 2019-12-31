using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public class CacheKeyTypeAttribue : Attribute
    {
        public CacheTypeEnum CacheTypeEnum
        {
            set;
            get;
        }

        public CacheKeyTypeAttribue(CacheTypeEnum cacheTypeEnum)
        {
            CacheTypeEnum = cacheTypeEnum;
        }
    }

    public class CacheDependencyAttibute : Attribute
    {
        public CacheDependencyAttibute()
        {
            CacheDependencys = new Dictionary<CacheDependencyEnum, CacheDependencyActionType[]>();
        }
        public Dictionary<CacheDependencyEnum, CacheDependencyActionType[]> CacheDependencys
        {
            set;
            get;
        }

        public CacheDependencyAttibute(CacheDependencyEnum[] cacheDimensionEnums, CacheDependencyActionType[] cacheDependencyActionTypes)
            : this()
        {
            foreach (CacheDependencyEnum item in cacheDimensionEnums)
            {
                CacheDependencys[item] = cacheDependencyActionTypes;
            }
        }

        public CacheDependencyAttibute(CacheDependencyEnum cacheDimensionEnum, CacheDependencyActionType[] cacheDependencyActionTypes)
            : this()
        {
            CacheDependencys[cacheDimensionEnum] = cacheDependencyActionTypes;
        }

        public CacheDependencyAttibute(string cacheDependencyStr)
            : this()
        {
            string[] segments = cacheDependencyStr.Split('|');
            foreach (string segment in segments)
            {
                string[] fields = segment.Split(new char[] { ':', ',' });

                CacheDependencyEnum cacheDependencyEnum = (CacheDependencyEnum)(Convert.ToInt32(fields[0]));
                CacheDependencys[cacheDependencyEnum] = new CacheDependencyActionType[fields.Length - 1];
                for (int index = 1; index < fields.Length; index++)
                {
                    CacheDependencyActionType cacheDependencyActionType = (CacheDependencyActionType)(Convert.ToInt32(fields[index]));
                    CacheDependencys[cacheDependencyEnum][index - 1] = cacheDependencyActionType;
                }
            }
        }
    }

    public class CacheDurationAttibute : Attribute
    {
        /// <summary>
        /// 单位：秒
        /// </summary>
        public int CacheDuration
        {
            set;
            get;
        }

        /// <summary>
        /// 单位：秒
        /// </summary>
        /// <param name="cacheDuration"></param>
        public CacheDurationAttibute(int cacheDuration)
        {
            CacheDuration = cacheDuration;
        }
    }
}
