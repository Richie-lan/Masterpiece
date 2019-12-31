using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    /// 缓存依赖项
    /// </summary>
    public enum CacheDependencyEnum
    {
        [Description("测试")]
        Test = 1,
        [Description("产品")]
        Product = 2
    }

    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheTypeEnum
    {
        Redis = 1,
        Sql = 2,
        File = 3,
        LocalCache = 4,
        Composite = 5,//本地Cache和Redis混合
    }

    /// <summary>
    /// 缓存依赖的操作类型
    /// </summary>
    public enum CacheDependencyActionType
    {
        Add = 1,
        Update = 2,
        Delete = 4,
        CUD = 8
    }

    public enum CacheStatusEnum
    {
        Enable = 1,//可用
        Disable = 2,//禁用
        Refresh = 3 //刷新
    }

    /// <summary>
    /// 缓存项
    /// </summary>
    public enum CacheKeyEnum
    {
        [Description("测试缓存")]
        [CacheDependencyAttibute(CacheDependencyEnum.Test, new CacheDependencyActionType[] { CacheDependencyActionType.Update, CacheDependencyActionType.Delete })]
        [CacheKeyTypeAttribue(CacheTypeEnum.Redis)]
        Test_Cache = 1,

        [Description("产品缓存")]
        [CacheDependencyAttibute(CacheDependencyEnum.Product, new CacheDependencyActionType[] { CacheDependencyActionType.Update, CacheDependencyActionType.Delete })]
        [CacheKeyTypeAttribue(CacheTypeEnum.Redis)]
        Product_Cache = 2
    }
}
