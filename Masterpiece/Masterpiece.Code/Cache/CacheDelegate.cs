using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{
    public delegate T GetObjInvokeDelegate<T, T1>(T1 id);

    public delegate T GetObjInvokeDelegate<T>();
}
