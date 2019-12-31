using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    /// <summary>
    /// 缓存依赖对象实体类
    /// </summary>
    public class CacheDependencyModel
    {
        public object Argument
        {
            set;
            get;
        }


        public CacheDependencyModel()
        {
        }

        public CacheDependencyModel(object argument)
            : this()
        {
            Argument = argument;
        }
    }
}
